using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;
using Windows.Win32.Media.MediaFoundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

// ReSharper disable once CheckNamespace
namespace Windows.Win32;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static partial class PInvoke
{
    public const uint MF_SOURCE_READER_FIRST_VIDEO_STREAM = 0xFFFFFFFCU;
    public const uint MF_SOURCE_READER_FIRST_AUDIO_STREAM = 0xFFFFFFFDU;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HRESULT MFGetAttributeSize(IMFAttributes pAttributes, out uint punWidth, out uint punHeight)
    {
        return MFGetAttribute2UINT32asUINT64(pAttributes, MF_MT_FRAME_SIZE, out punWidth, out punHeight);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static HRESULT MFGetAttribute2UINT32asUINT64(IMFAttributes pAttributes, in Guid guidKey, out uint punHigh32, out uint punLow32)
    {
        var result = pAttributes.GetUINT64(guidKey, out var unPacked);

        if (result.Value < 0)
        {
            punHigh32 = 0;
            punLow32 = 0;

            return result;
        }

        Unpack2UINT32AsUINT64(unPacked, out punHigh32, out punLow32);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Unpack2UINT32AsUINT64(ulong unPacked, out uint punHigh, out uint punLow)
    {
        punHigh = (uint)(unPacked >> 32);
        punLow = (uint)unPacked;
    }
}
