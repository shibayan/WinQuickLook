using System;

namespace WinQuickLook.Interop
{
    public static class Consts
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int HC_ACTION = 0;
        public const int WM_KEYDOWN = 0x0100;

        public const int MAX_PATH = 260;
    }

    [Flags]
    public enum SHGFI : uint
    {
        ICON = 0x000000100,
        DISPLAYNAME = 0x000000200,
        TYPENAME = 0x000000400,
        ATTRIBUTES = 0x000000800,
        ICONLOCATION = 0x000001000,
        EXETYPE = 0x000002000,
        SYSICONINDEX = 0x000004000,
        LINKOVERLAY = 0x000008000,
        SELECTED = 0x000010000,
        ATTR_SPECIFIED = 0x000020000,
        LARGEICON = 0x000000000,
        SMALLICON = 0x000000001,
        OPENICON = 0x000000002,
        SHELLICONSIZE = 0x000000004,
        PIDL = 0x000000008,
        USEFILEATTRIBUTES = 0x000000010,
        ADDOVERLAYS = 0x000000020,
        OVERLAYINDEX = 0x000000040,
    }
}
