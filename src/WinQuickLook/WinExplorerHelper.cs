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
                object dispatch;

                var pvarLoc = new object();

                shellWindows.FindWindowSW(ref pvarLoc, ref pvarLoc, ShellWindowTypeConstants.SWC_DESKTOP, out desktopHwnd, ShellWindowFindWindowOptions.SWFO_NEEDDISPATCH, out dispatch);

                if (!IsCaretActive(desktopHwnd))
                {
                    var webBrowserApp = (IWebBrowserApp)dispatch;

                    fileName = GetSelectedItemCore(webBrowserApp);

                    Marshal.ReleaseComObject(webBrowserApp);
                }

                Marshal.ReleaseComObject(dispatch);
            }
            else
            {
                for (int i = 0; i < shellWindows.Count && fileName == null ; i++)
                {
                    var dispatch = shellWindows.Item(i);

                    var webBrowserApp = (IWebBrowserApp)dispatch;

                    var hwnd = webBrowserApp.get_HWND();

                    if (hwnd == foregroundHwnd && !IsCaretActive(hwnd))
                    {
                        fileName = GetSelectedItemCore(webBrowserApp);
                    }

                    Marshal.ReleaseComObject(webBrowserApp);
                    Marshal.ReleaseComObject(dispatch);
                }
            }

            Marshal.ReleaseComObject(shellWindows);

            return fileName;
        }

        public static void CreateShortcutLink(string linkPath)
        {
            var shellLink = (IShellLink)Activator.CreateInstance(CLSID.ShellLinkType);
            var persistFile = shellLink.QueryInterface<IPersistFile>();

            shellLink.SetPath(Assembly.GetEntryAssembly().Location);
            persistFile.Save(linkPath, true);

            Marshal.ReleaseComObject(persistFile);
            Marshal.ReleaseComObject(shellLink);
        }

        private static string GetSelectedItemCore(IWebBrowserApp webBrowserApp)
        {
            var serviceProvider = webBrowserApp.QueryInterface<Interop.IServiceProvider>();

            var shellBrowser = serviceProvider.QueryService<IShellBrowser>(SID.STopLevelBrowser);

            var shellView = shellBrowser.QueryActiveShellView();

            var folderView = shellView.QueryInterface<IFolderView>();

            int focus = folderView.GetFocusedItem();

            var pidl = folderView.Item(focus);

            var persistFolder2 = folderView.GetFolder<IPersistFolder2>();

            var shellFolder = persistFolder2.QueryInterface<IShellFolder>();

            var filename = shellFolder.GetDisplayNameOf(pidl, SHGDNF.FORPARSING);

            // Cleanup
            Marshal.FreeCoTaskMem(pidl);

            Marshal.ReleaseComObject(shellFolder);
            Marshal.ReleaseComObject(persistFolder2);
            Marshal.ReleaseComObject(folderView);
            Marshal.ReleaseComObject(shellView);
            Marshal.ReleaseComObject(shellBrowser);
            Marshal.ReleaseComObject(serviceProvider);

            return filename;
        }

        private static bool IsCaretActive(IntPtr hwnd)
        {
            var threadId = NativeMethods.GetWindowThreadProcessId(hwnd, IntPtr.Zero);

            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);

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
