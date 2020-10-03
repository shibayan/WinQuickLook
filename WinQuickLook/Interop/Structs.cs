using System;
using System.Runtime.InteropServices;

namespace WinQuickLook.Interop
{
#if X86
    [StructLayout(LayoutKind.Explicit, Size = 264)]
    public struct STRRET
    {
        [FieldOffset(0)]
        public uint uType;

        [FieldOffset(4)]
        public IntPtr pOleStr;

        [FieldOffset(4)]
        public uint uOffset;

        [FieldOffset(4)]
        public IntPtr cStr;
    }
#else
    [StructLayout(LayoutKind.Explicit, Size = 272)]
    public struct STRRET
    {
        [FieldOffset(0)]
        public uint uType;

        [FieldOffset(8)]
        public IntPtr pOleStr;

        [FieldOffset(8)]
        public uint uOffset;

        [FieldOffset(8)]
        public IntPtr cStr;
    }
#endif

    [StructLayout(LayoutKind.Sequential)]
    public struct GUITHREADINFO
    {
        public int cbSize;
        public int flags;
        public IntPtr hwndActive;
        public IntPtr hwndFocus;
        public IntPtr hwndCapture;
        public IntPtr hwndMenuOwner;
        public IntPtr hwndMoveSize;
        public IntPtr hwndCaret;
        public RECT rcCaret;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;

        public SIZE(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;

        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int x;
        public int y;
        public int cx;
        public int cy;

        public RECT(int x, int y, int cx, int cy)
        {
            this.x = x;
            this.y = y;
            this.cx = cx;
            this.cy = cy;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DetectEncodingInfo
    {
        public int nLangID;
        public int nCodePage;
        public int nDocPercent;
        public int nConfidence;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Consts.MAX_PATH)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACCENTPOLICY
    {
        public int nAccentState;
        public int nFlags;
        public uint nColor;
        public int nAnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINCOMPATTRDATA
    {
        public int nAttribute;
        public IntPtr pData;
        public int ulDataSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public int dwFlags;
    }
}
