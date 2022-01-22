using System;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;

namespace Windows.Win32;

public static partial class PInvoke
{
    public static unsafe HRESULT SHCreateItemFromParsingName<T>(string pszPath, System.Com.IBindCtx pbc, out T ppv)
    {
        var result = SHCreateItemFromParsingName(pszPath, pbc, typeof(T).GUID, out var o);
        ppv = (T)Marshal.GetUniqueObjectForIUnknown(new IntPtr(o));
        return result;
    }

    public static unsafe HRESULT AssocQueryString(uint flags, UI.Shell.ASSOCSTR str, string pszAssoc, string pszExtra, Span<char> pszOut, ref uint pcchOut)
    {
        fixed (char* p = pszOut)
        {
            return AssocQueryString(flags, str, pszAssoc, pszExtra, new PWSTR(p), ref pcchOut);
        }
    }
}
