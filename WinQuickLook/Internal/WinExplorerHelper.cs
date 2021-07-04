using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Interop;

using IServiceProvider = WinQuickLook.Interop.IServiceProvider;

namespace WinQuickLook.Internal
{
    public static class WinExplorerHelper
    {
        private const long TeraByte = 1024L * 1024 * 1024 * 1024;
        private const long GigaByte = 1024L * 1024 * 1024;
        private const long MegaByte = 1024L * 1024;
        private const long KiroByte = 1024L;

        public static string GetSizeFormat(long length)
        {
            if (length >= TeraByte)
            {
                return $"{length / (double)TeraByte:0.##} TB";
            }

            if (length >= GigaByte)
            {
                return $"{length / (double)GigaByte:0.##} GB";
            }

            if (length >= MegaByte)
            {
                return $"{length / (double)MegaByte:0.##} MB";
            }

            if (length >= KiroByte)
            {
                return $"{length / (double)KiroByte:0.##} KB";
            }

            return $"{length} B";
        }

        public static string GetAssocName(string fileName)
        {
            var pcchOut = 0;

            NativeMethods.AssocQueryString(ASSOCF.INIT_IGNOREUNKNOWN, ASSOCSTR.FRIENDLYAPPNAME, Path.GetExtension(fileName), null, null, ref pcchOut);

            if (pcchOut == 0)
            {
                return null;
            }

            var pszOut = new StringBuilder(pcchOut);

            NativeMethods.AssocQueryString(ASSOCF.INIT_IGNOREUNKNOWN, ASSOCSTR.FRIENDLYAPPNAME, Path.GetExtension(fileName), null, pszOut, ref pcchOut);

            return pszOut.ToString().Trim();
        }

        public class AssocAppEntry
        {
            public string Name { get; set; }
            public ImageSource Icon { get; set; }
        }

        public static IList<AssocAppEntry> GetAssocAppList(string fileName)
        {
            var list = new List<AssocAppEntry>();

            NativeMethods.SHAssocEnumHandlers(Path.GetExtension(fileName), ASSOC_FILTER.RECOMMENDED, out var enumAssocHandlers);

            while (enumAssocHandlers.Next(1, out var assocHandler, out _) == 0)
            {
                if (assocHandler == null)
                {
                    break;
                }

                assocHandler.GetUIName(out var uiName);
                assocHandler.GetIconLocation(out var location, out var index);

                var icons = new IntPtr[1];

                NativeMethods.ExtractIconEx(location, index, null, icons, 1);

                list.Add(new AssocAppEntry
                {
                    Name = uiName,
                    Icon = Imaging.CreateBitmapSourceFromHIcon(icons[0], Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                });

                NativeMethods.DestroyIcon(icons[0]);

                Marshal.ReleaseComObject(assocHandler);
            }

            Marshal.ReleaseComObject(enumAssocHandlers);

            return list;
        }

        public static string GetSelectedItem()
        {
            var foregroundHwnd = NativeMethods.GetForegroundWindow();

            if (foregroundHwnd == IntPtr.Zero)
            {
                return null;
            }

            var shellWindows = (IShellWindows)Activator.CreateInstance(CLSID.ShellWindowsType);

            string fileName = null;

            if (IsDesktopWindow(foregroundHwnd))
            {
                var pvarLoc = new object();

                shellWindows.FindWindowSW(ref pvarLoc, ref pvarLoc, ShellWindowTypeConstants.SWC_DESKTOP, out var desktopHwnd, ShellWindowFindWindowOptions.SWFO_NEEDDISPATCH, out var webBrowserApp);

                if (!IsCaretActive(desktopHwnd))
                {
                    fileName = GetSelectedItemCore(webBrowserApp);
                }

                Marshal.FinalReleaseComObject(webBrowserApp);
            }
            else
            {
                for (int i = 0; i < shellWindows.Count; i++)
                {
                    var webBrowserApp = (IWebBrowserApp)shellWindows.Item(i);

                    var hwnd = webBrowserApp.get_HWND();

                    if (hwnd == foregroundHwnd && !IsCaretActive(hwnd))
                    {
                        fileName = GetSelectedItemCore(webBrowserApp);

                        Marshal.FinalReleaseComObject(webBrowserApp);

                        break;
                    }

                    Marshal.FinalReleaseComObject(webBrowserApp);
                }
            }

            Marshal.FinalReleaseComObject(shellWindows);

            if (fileName == null || (!File.Exists(fileName) && !Directory.Exists(fileName)))
            {
                return null;
            }

            return fileName;
        }

        private static string GetSelectedItemCore(IWebBrowserApp webBrowserApp)
        {
            var serviceProvider = webBrowserApp.QueryInterface<IServiceProvider>();

            var shellBrowser = serviceProvider.QueryService<IShellBrowser>(SID.STopLevelBrowser);

            var shellView = shellBrowser.QueryActiveShellView();

            var folderView = shellView.QueryInterface<IFolderView>();

            int focus = folderView.GetFocusedItem();

            var persistFolder2 = folderView.GetFolder<IPersistFolder2>();

            var shellFolder = persistFolder2.QueryInterface<IShellFolder>();

            var pidl = folderView.Item(focus);

            var fileName = shellFolder.GetDisplayNameOf(pidl, SHGDNF.FORPARSING);

            // Cleanup
            Marshal.FreeCoTaskMem(pidl);

            Marshal.FinalReleaseComObject(shellFolder);
            Marshal.FinalReleaseComObject(persistFolder2);
            Marshal.FinalReleaseComObject(folderView);
            Marshal.FinalReleaseComObject(shellView);
            Marshal.FinalReleaseComObject(shellBrowser);
            Marshal.FinalReleaseComObject(serviceProvider);

            return fileName;
        }

        private static bool IsCaretActive(IntPtr hwnd)
        {
            var threadId = NativeMethods.GetWindowThreadProcessId(hwnd, IntPtr.Zero);

            var info = new GUITHREADINFO
            {
                cbSize = Marshal.SizeOf<GUITHREADINFO>()
            };

            NativeMethods.GetGUIThreadInfo(threadId, ref info);

            return info.flags != 0 || info.hwndCaret != IntPtr.Zero;
        }

        private static bool IsDesktopWindow(IntPtr hwnd)
        {
            var classNameBuilder = new StringBuilder(Consts.MAX_PATH);

            NativeMethods.GetClassName(hwnd, classNameBuilder, Consts.MAX_PATH);

            var className = classNameBuilder.ToString();

            if (className != "Progman" && className != "WorkerW")
            {
                return false;
            }

            return NativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero;
        }
    }
}
