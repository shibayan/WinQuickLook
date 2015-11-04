using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using WinQuickLook.Interop;

namespace WinQuickLook
{
    public static class WinExplorerHelper
    {
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
                IntPtr desktopHwnd;
                IWebBrowserApp webBrowserApp;

                var pvarLoc = new object();

                shellWindows.FindWindowSW(ref pvarLoc, ref pvarLoc, ShellWindowTypeConstants.SWC_DESKTOP, out desktopHwnd, ShellWindowFindWindowOptions.SWFO_NEEDDISPATCH, out webBrowserApp);

                if (!IsCaretActive(desktopHwnd))
                {
                    fileName = GetSelectedItemCore(webBrowserApp);
                }
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

                        break;
                    }
                }
            }

            Marshal.FinalReleaseComObject(shellWindows);

            return fileName;
        }

        public static void CreateShortcutLink(string linkPath)
        {
            var shellLink = (IShellLink)Activator.CreateInstance(CLSID.ShellLinkType);
            var persistFile = shellLink.QueryInterface<IPersistFile>();

            shellLink.SetPath(Assembly.GetEntryAssembly().Location);
            persistFile.Save(linkPath, true);

            Marshal.FinalReleaseComObject(shellLink);
        }

        private static string GetSelectedItemCore(IWebBrowserApp webBrowserApp)
        {
            var serviceProvider = webBrowserApp.QueryInterface<Interop.IServiceProvider>();

            var shellBrowser = serviceProvider.QueryService<IShellBrowser>(SID.STopLevelBrowser);

            var shellView = shellBrowser.QueryActiveShellView();

            var folderView = shellView.QueryInterface<IFolderView>();

            int focus = folderView.GetFocusedItem();

            var persistFolder2 = folderView.GetFolder<IPersistFolder2>();

            var shellFolder = persistFolder2.QueryInterface<IShellFolder>();

            var pidl = folderView.Item(focus);

            var filename = shellFolder.GetDisplayNameOf(pidl, SHGDNF.FORPARSING);

            // Cleanup
            Marshal.FreeCoTaskMem(pidl);

            Marshal.FinalReleaseComObject(shellFolder);
            Marshal.FinalReleaseComObject(folderView);
            Marshal.FinalReleaseComObject(shellBrowser);
            Marshal.FinalReleaseComObject(serviceProvider);

            return filename;
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
