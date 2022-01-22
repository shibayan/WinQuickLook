using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Windows.Win32;

public static partial class PInvoke
{
    [SupportedOSPlatform("windows6.0.6000")]
    public static unsafe Foundation.HRESULT SHCreateItemFromParsingName<T>(string pszPath, System.Com.IBindCtx pbc, out T ppv)
    {
        var result = SHCreateItemFromParsingName(pszPath, pbc, typeof(T).GUID, out var o);
        ppv = (T)Marshal.GetUniqueObjectForIUnknown(new IntPtr(o));
        return result;
    }
}
