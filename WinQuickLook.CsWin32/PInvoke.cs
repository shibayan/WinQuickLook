using System;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Windows.Win32;

public static partial class PInvoke
{
    public static unsafe HRESULT SHCreateItemFromParsingName<T>(string pszPath, System.Com.IBindCtx? pbc, out T ppv)
    {
        var result = SHCreateItemFromParsingName(pszPath, pbc, typeof(T).GUID, out var o);
        ppv = (T)Marshal.GetUniqueObjectForIUnknown(new IntPtr(o));
        return result;
    }

    public static unsafe HRESULT AssocQueryString(uint flags, ASSOCSTR str, string pszAssoc, string pszExtra, Span<char> pszOut, ref uint pcchOut)
    {
        fixed (char* p = pszOut)
        {
            return AssocQueryString(flags, str, pszAssoc, pszExtra, new PWSTR(p), ref pcchOut);
        }
    }

    public static unsafe HWND CreateWindowEx(WINDOW_EX_STYLE dwExStyle, string lpClassName, string lpWindowName, WINDOW_STYLE dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, SafeHandle? hMenu = default, SafeHandle? hInstance = default)
    {
        return CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, null);
    }
}
