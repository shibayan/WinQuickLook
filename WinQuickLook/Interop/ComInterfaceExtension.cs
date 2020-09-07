using System;

namespace WinQuickLook.Interop
{
    public static class ComInterfaceExtension
    {
        public static TInterface QueryInterface<TInterface>(this IUnknown obj)
        {
            try
            {
                return (TInterface)obj;
            }
            catch
            {
                return default;
            }
        }

        public static TInterface QueryService<TInterface>(this IServiceProvider serviceProvider, Guid guidService)
        {
            serviceProvider.QueryService(guidService, typeof(TInterface).GUID, out var ppvObject);

            return (TInterface)ppvObject;
        }

        public static IShellView QueryActiveShellView(this IShellBrowser shellBrowser)
        {
            shellBrowser.QueryActiveShellView(out var ppshv);

            return ppshv;
        }

        public static TInterface GetFolder<TInterface>(this IFolderView folderView)
        {
            folderView.GetFolder(typeof(TInterface).GUID, out var ppv);

            return (TInterface)ppv;
        }

        public static int GetFocusedItem(this IFolderView folderView)
        {
            folderView.GetFocusedItem(out var piItem);

            return piItem;
        }

        public static IntPtr Item(this IFolderView folderView, int index)
        {
            folderView.Item(index, out var pidl);

            return pidl;
        }

        public static string GetDisplayNameOf(this IShellFolder shellFolder, IntPtr pidl, SHGDNF uFlags)
        {
            shellFolder.GetDisplayNameOf(pidl, SHGDNF.FORPARSING, out var str);

            NativeMethods.StrRetToBSTR(ref str, pidl, out var buffer);

            return buffer;
        }
    }
}
