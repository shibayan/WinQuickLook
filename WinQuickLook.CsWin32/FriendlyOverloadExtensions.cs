using System;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;

using IServiceProvider = Windows.Win32.System.Com.IServiceProvider;

namespace Windows.Win32;

public static partial class FriendlyOverloadExtensions
{
    public static unsafe HRESULT QueryService<T>(this IServiceProvider serviceProvider, in Guid guidService, out T ppvObject)
    {
        var hr = serviceProvider.QueryService(guidService, typeof(T).GUID, out var o);
        ppvObject = (T)Marshal.GetUniqueObjectForIUnknown(new IntPtr(o));
        return hr;
    }

    public static HRESULT GetFolder<T>(this IFolderView folderView, out T ppv)
    {
        var hr = folderView.GetFolder(typeof(T).GUID, out var o);
        ppv = (T)o;
        return hr;
    }

    public static unsafe HRESULT Next(this IEnumAssocHandlers enumAssocHandlers, IAssocHandler[] rgelt, out uint pceltFetched)
    {
        fixed (uint* pceltFetchedLocal = &pceltFetched)
        {
            return enumAssocHandlers.Next((uint)rgelt.Length, rgelt, pceltFetchedLocal);
        }
    }
}
