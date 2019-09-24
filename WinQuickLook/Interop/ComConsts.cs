using System;

namespace WinQuickLook.Interop
{
    public static class CLSID
    {
        public static readonly Guid ShellWindows = new Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39");
        public static readonly Guid ShellLink = new Guid("00021401-0000-0000-C000-000000000046");
        public static readonly Guid MultiLanguage = new Guid("275c23e2-3747-11d0-9fea-00aa003f8646");

        public static readonly Type ShellWindowsType = Type.GetTypeFromCLSID(ShellWindows);
        public static readonly Type ShellLinkType = Type.GetTypeFromCLSID(ShellLink);
        public static readonly Type MultiLanguageType = Type.GetTypeFromCLSID(MultiLanguage);
    }

    public static class SID
    {
        public static readonly Guid STopLevelBrowser = new Guid(0x4C96BE40, 0x915C, 0x11CF, 0x99, 0xD3, 0x00, 0xAA, 0x00, 0x4A, 0xE8, 0x37);
    }

    [Flags]
    public enum SIIGBF : uint
    {
        RESIZETOFIT = 0,
        BIGGERSIZEOK = 0x1,
        MEMORYONLY = 0x2,
        ICONONLY = 0x4,
        THUMBNAILONLY = 0x8,
        INCACHEONLY = 0x10,
        CROPTOSQUARE = 0x20,
        WIDETHUMBNAILS = 0x40,
        ICONBACKGROUND = 0x80,
        SCALEUP = 0x100
    }

    public enum SIGDN : uint
    {
        NORMALDISPLAY = 0,
        PARENTRELATIVEPARSING = 0x80018001,
        DESKTOPABSOLUTEPARSING = 0x80028000,
        PARENTRELATIVEEDITING = 0x80031001,
        DESKTOPABSOLUTEEDITING = 0x8004c000,
        FILESYSPATH = 0x80058000,
        URL = 0x80068000,
        PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
        PARENTRELATIVE = 0x80080001,
        PARENTRELATIVEFORUI = 0x80094001
    }

    [Flags]
    public enum SHGDNF : uint
    {
        NORMAL = 0,
        INFOLDER = 0x1,
        FOREDITING = 0x1000,
        FORADDRESSBAR = 0x4000,
        FORPARSING = 0x8000
    }

    [Flags]
    public enum ShellWindowTypeConstants
    {
        SWC_EXPLORER = 0x0,
        SWC_BROWSER = 0x00000001,
        SWC_3RDPARTY = 0x00000002,
        SWC_CALLBACK = 0x00000004,
        SWC_DESKTOP = 0x00000008
    }

    [Flags]
    public enum ShellWindowFindWindowOptions
    {
        SWFO_NEEDDISPATCH = 0x00000001,
        SWFO_INCLUDEPENDING = 0x00000002,
        SWFO_COOKIEPASSED = 0x00000004
    }
}
