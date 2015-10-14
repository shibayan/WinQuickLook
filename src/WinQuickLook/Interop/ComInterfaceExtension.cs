using System;

namespace WinQuickLook.Interop
{
    public static class ComInterfaceExtension
    {
        public static TInterface QueryInterface<TInterface>(this IUnknown obj)
        {
            return (TInterface)obj;
        }

        public static TInterface QueryService<TInterface>(this IServiceProvider serviceProvider, Guid guidService)
        {
            object ppvObject;

            serviceProvider.QueryService(guidService, typeof(TInterface).GUID, out ppvObject);

            return (TInterface)ppvObject;
        }

        public static IShellView QueryActiveShellView(this IShellBrowser shellBrowser)
        {
            IShellView ppshv;

            shellBrowser.QueryActiveShellView(out ppshv);

            return ppshv;
        }

        public static TInterface GetFolder<TInterface>(this IFolderView folderView)
        {
            object ppv;

            folderView.GetFolder(typeof(TInterface).GUID, out ppv);

            return (TInterface)ppv;
        }

        public static int GetFocusedItem(this IFolderView folderView)
        {
            int piItem;

            folderView.GetFocusedItem(out piItem);

            return piItem;
        }

        public static IntPtr Item(this IFolderView folderView, int index)
        {
            IntPtr pidl;

            folderView.Item(index, out pidl);

            return pidl;
        }

        public static string GetDisplayNameOf(this IShellFolder shellFolder, IntPtr pidl, SHGDNF uFlags)
        {
            STRRET str;

            shellFolder.GetDisplayNameOf(pidl, SHGDNF.FORPARSING, out str);

            string buffer;

            NativeMethods.StrRetToBSTR(ref str, pidl, out buffer);

            return buffer;
        }
    }
}
