using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;
using Windows.Win32.Media.MediaFoundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.Common;
using Windows.Win32.UI.Shell.PropertiesSystem;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Windows.Win32;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static partial class PInvoke
{
    public const uint MF_VERSION = 0x0001 << 16 | 0x0070;

    public const uint MF_SOURCE_READER_FIRST_VIDEO_STREAM = 0xFFFFFFFCU;
    public const uint MF_SOURCE_READER_FIRST_AUDIO_STREAM = 0xFFFFFFFDU;

    public static HRESULT SHCreateItemFromParsingName<T>(string pszPath, System.Com.IBindCtx pbc, out T ppv)
    {
        var hr = SHCreateItemFromParsingName(pszPath, pbc, typeof(T).GUID, out var o);
        ppv = (T)o;
        return hr;
    }

    public static unsafe HRESULT SHGetPropertyStoreFromParsingName<T>(string pszPath, System.Com.IBindCtx pbc, GETPROPERTYSTOREFLAGS flags, out T ppv)
    {
        var hr = SHGetPropertyStoreFromParsingName(pszPath, pbc, flags, typeof(T).GUID, out var o);
        ppv = (T)Marshal.GetTypedObjectForIUnknown(new IntPtr(o), typeof(T));
        return hr;
    }

    /// <inheritdoc cref="AssocQueryString(uint, ASSOCSTR, PCWSTR, PCWSTR, PWSTR, uint*)"/>
    public static unsafe HRESULT AssocQueryString(ASSOCF flags, ASSOCSTR str, string pszAssoc, string pszExtra, Span<char> pszOut, ref uint pcchOut)
    {
        fixed (char* pszOutLocal = pszOut)
        {
            return AssocQueryString((uint)flags, str, pszAssoc, pszExtra, new PWSTR(pszOutLocal), ref pcchOut);
        }
    }

    public static unsafe HWND CreateWindowEx(WINDOW_EX_STYLE dwExStyle, string lpClassName, string lpWindowName, WINDOW_STYLE dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, SafeHandle hMenu = default, SafeHandle hInstance = default)
    {
        return CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, null);
    }

    /// <inheritdoc cref="SHLoadIndirectString(PCWSTR, PWSTR, uint, void**)"/>
    public static unsafe HRESULT SHLoadIndirectString(string pszSource, Span<char> pszOutBuf)
    {
        fixed (char* pszOutBufLocal = pszOutBuf)
        {
            fixed (char* pszSourceLocal = pszSource)
            {
                return SHLoadIndirectString(pszSourceLocal, new PWSTR(pszOutBufLocal), (uint)pszOutBuf.Length, null);
            }
        }
    }

    public static unsafe HRESULT StrRetToBuf(ref STRRET pstr, IntPtr pidl, Span<char> pszBuf)
    {
        fixed (STRRET* pstrLocal = &pstr)
        {
            fixed (char* pszBufLocal = pszBuf)
            {
                return StrRetToBuf(pstrLocal, (ITEMIDLIST*)pidl, new PWSTR(pszBufLocal), (uint)pszBuf.Length);
            }
        }
    }

    public static unsafe uint GetWindowThreadProcessId(HWND hWnd) => GetWindowThreadProcessId(hWnd, null);

    public static unsafe int GetClassName(HWND hWnd, Span<char> lpClassName)
    {
        fixed (char* lpClassNameLocal = lpClassName)
        {
            return GetClassName(hWnd, new PWSTR(lpClassNameLocal), lpClassName.Length);
        }
    }

    public static unsafe HRESULT DwmSetWindowAttribute<T>(HWND hwnd, Graphics.Dwm.DWMWINDOWATTRIBUTE dwAttribute, ref T pvAttribute) where T : unmanaged
    {
        fixed (T* pvAttributeLocal = &pvAttribute)
        {
            return DwmSetWindowAttribute(hwnd, dwAttribute, pvAttributeLocal, (uint)Marshal.SizeOf<T>());
        }
    }

    public static unsafe nuint SHGetFileInfo(string pszPath, Storage.FileSystem.FILE_FLAGS_AND_ATTRIBUTES dwFileAttributes, ref SHFILEINFOW psfi, SHGFI_FLAGS uFlags)
    {
        fixed (char* pszPathLocal = pszPath)
        {
            fixed (SHFILEINFOW* psfiLocal = &psfi)
            {
                return SHGetFileInfo(pszPathLocal, dwFileAttributes, psfiLocal, (uint)Marshal.SizeOf<SHFILEINFOW>(), uFlags);
            }
        }
    }

    [DllImport("Ole32", ExactSpelling = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern HRESULT PropVariantClear(ref PROPVARIANT pvar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HRESULT MFGetAttributeSize(IMFAttributes pAttributes, out uint punWidth, out uint punHeight)
    {
        return MFGetAttribute2UINT32asUINT64(pAttributes, MF_MT_FRAME_SIZE, out punWidth, out punHeight);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static HRESULT MFGetAttribute2UINT32asUINT64(IMFAttributes pAttributes, in Guid guidKey, out uint punHigh32, out uint punLow32)
    {
        var hr = pAttributes.GetUINT64(guidKey, out var unPacked);

        if (hr.Value < 0)
        {
            punHigh32 = 0;
            punLow32 = 0;

            return hr;
        }

        Unpack2UINT32AsUINT64(unPacked, out punHigh32, out punLow32);

        return hr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Unpack2UINT32AsUINT64(ulong unPacked, out uint punHigh, out uint punLow)
    {
        punHigh = (uint)(unPacked >> 32);
        punLow = (uint)unPacked;
    }
}
