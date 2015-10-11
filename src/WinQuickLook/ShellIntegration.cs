using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Interop;

namespace WinQuickLook
{
    public class ShellIntegration
    {
        public string GetSelectedItem()
        {
            var foregroundHwnd = NativeMethods.GetForegroundWindow();

            var shellWindows = (IShellWindows)Activator.CreateInstance(CLSID.ShellWindowsType);

            string filename = null;

            for (int i = 0; i < shellWindows.Count; i++)
            {
                var webBrowserApp = (IWebBrowserApp)shellWindows.Item(i);

                var hwnd = webBrowserApp.get_HWND();

                if (hwnd != foregroundHwnd || IsCaretActive(hwnd))
                {
                    Marshal.ReleaseComObject(webBrowserApp);

                    continue;
                }

                var serviceProvider = webBrowserApp.QueryInterface<Interop.IServiceProvider>();

                var shellBrowser = serviceProvider.QueryService<IShellBrowser>(SID.STopLevelBrowser);

                var shellView = shellBrowser.QueryActiveShellView();

                var folderView = shellView.QueryInterface<IFolderView>();

                int focus = folderView.GetFocusedItem();

                var pidl = folderView.Item(focus);

                var persistFolder2 = folderView.GetFolder<IPersistFolder2>();

                var shellFolder = persistFolder2.QueryInterface<IShellFolder>();

                filename = shellFolder.GetDisplayNameOf(pidl, SHGDNF.FORPARSING);

                // Cleanup
                Marshal.FreeCoTaskMem(pidl);

                Marshal.ReleaseComObject(shellFolder);
                Marshal.ReleaseComObject(persistFolder2);
                Marshal.ReleaseComObject(folderView);
                Marshal.ReleaseComObject(shellView);
                Marshal.ReleaseComObject(shellBrowser);
                Marshal.ReleaseComObject(serviceProvider);
                Marshal.ReleaseComObject(webBrowserApp);

                break;
            }

            Marshal.ReleaseComObject(shellWindows);

            return filename;
        }

        public ImageSource GetThumbnail(string fileName)
        {
            IShellItem shellItem;

            NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, typeof(IShellItem).GUID, out shellItem);

            var imageFactory = shellItem.QueryInterface<IShellItemImageFactory>();

            IntPtr bitmap;

            imageFactory.GetImage(new SIZE(256, 256), SIIGBF.RESIZETOFIT, out bitmap);

            var image = Imaging.CreateBitmapSourceFromHBitmap(bitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            NativeMethods.DeleteObject(bitmap);

            Marshal.ReleaseComObject(imageFactory);
            Marshal.ReleaseComObject(shellItem);

            return image;
        }

        private static bool IsCaretActive(IntPtr hwnd)
        {
            var threadId = NativeMethods.GetWindowThreadProcessId(hwnd, IntPtr.Zero);

            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);

            NativeMethods.GetGUIThreadInfo(threadId, ref info);

            return info.flags != 0 || info.hwndCaret != IntPtr.Zero;
        }
    }
}
