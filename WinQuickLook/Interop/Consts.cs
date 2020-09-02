using System;

namespace WinQuickLook.Interop
{
    public static class Consts
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE_LL = 14;
        public const int HC_ACTION = 0;

        public const int HTCAPTION = 0x02;
        public const int WM_NCRBUTTONDOWN = 0x00a4;
        public const int WM_CONTEXTMENU = 0x007b;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_LBUTTONDOWN = 0x0201;

        public const int VK_F4 = 0x73;

        public const int MAX_PATH = 260;

        public const int GWL_STYLE = -16;
        public const int WS_MAXIMIZEBOX = 0x00010000;
        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_SYSMENU = 0x00080000;

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x8000000;

        public const int MONITOR_DEFAULTTOPRIMARY = 1;
        public const int MONITOR_DEFAULTTONEAREST = 2;

        public const int DPI_AWARENESS_CONTEXT_UNAWARE = 16;
        public const int DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = 17;
        public const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = 18;
        public const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = 34;

        public const int MDT_EFFECTIVE_DPI = 0;
        public const int MDT_ANGULAR_DPI = 1;
        public const int MDT_RAW_API = 3;

        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOZORDER = 0x0004;
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

    [Flags]
    public enum ASSOCF : uint
    {
        NONE = 0x00000000,
        INIT_NOREMAPCLSID = 0x00000001,
        INIT_BYEXENAME = 0x00000002,
        OPEN_BYEXENAME = 0x00000002,
        INIT_DEFAULTTOSTAR = 0x00000004,
        INIT_DEFAULTTOFOLDER = 0x00000008,
        NOUSERSETTINGS = 0x00000010,
        NOTRUNCATE = 0x00000020,
        VERIFY = 0x00000040,
        REMAPRUNDLL = 0x00000080,
        NOFIXUPS = 0x00000100,
        IGNOREBASECLASS = 0x00000200,
        INIT_IGNOREUNKNOWN = 0x00000400,
        INIT_FIXED_PROGID = 0x00000800,
        IS_PROTOCOL = 0x00001000,
        INIT_FOR_FILE = 0x00002000
    }

    public enum ASSOCSTR : uint
    {
        COMMAND = 1,
        EXECUTABLE,
        FRIENDLYDOCNAME,
        FRIENDLYAPPNAME,
        NOOPEN,
        SHELLNEWVALUE,
        DDECOMMAND,
        DDEIFEXEC,
        DDEAPPLICATION,
        DDETOPIC,
        INFOTIP,
        QUICKTIP,
        TILEINFO,
        CONTENTTYPE,
        DEFAULTICON,
        SHELLEXTENSION,
        DROPTARGET,
        DELEGATEEXECUTE,
        SUPPORTED_URI_PROTOCOLS,
        PROGID,
        APPID,
        APPPUBLISHER,
        APPICONREFERENCE,
        MAX
    }
}
