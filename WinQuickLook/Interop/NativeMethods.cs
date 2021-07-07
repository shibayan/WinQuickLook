using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace WinQuickLook.Interop
{
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr LowLevelHookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelHookProc lpfn, IntPtr hmod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        public static extern void SHCreateItemFromParsingName([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)]
                                                              Guid riid, [Out, MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)]
                                                              out IShellItem ppv);

        [DllImport("shell32.dll")]
        public static extern long SHAssocEnumHandlers([In, MarshalAs(UnmanagedType.LPWStr)] string pszExtra, [In] ASSOC_FILTER afFilter, [Out, MarshalAs(UnmanagedType.Interface)] out IEnumAssocHandlers ppEnumHandler);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo([In, MarshalAs(UnmanagedType.LPTStr)] string pszPath, uint dwFileAttributes, [In, Out] ref SHFILEINFO psfi, int cbFileInfo, SHGFI uFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx([In, MarshalAs(UnmanagedType.LPTStr)] string lpszFile, int nIconIndex, [In, Out] IntPtr[] phiconLarge, [In, Out] IntPtr[] phiconSmall, uint nIcons);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon([In] IntPtr hIcon);

        [DllImport("shlwapi.dll")]
        public static extern void StrRetToBSTR([In, Out] ref STRRET pstr, [In] IntPtr pidl, [Out, MarshalAs(UnmanagedType.BStr)] out string pbstr);

        [DllImport("shlwapi.dll")]
        public static extern int SHLoadIndirectString([In, MarshalAs(UnmanagedType.LPWStr)] string pszSource, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszOutBuf, int cchOuutBuf, IntPtr ppvReserved);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern uint GetGUIThreadInfo(uint dwthreadid, ref GUITHREADINFO lpguithreadinfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINCOMPATTRDATA data);

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern uint AssocQueryString(ASSOCF flags, ASSOCSTR str, string pszAssoc, string pszExtra, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszOut, [In, Out] ref int pcchOut);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool pfEnabled);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessDpiAwarenessContext(int dpiFlag);

        [DllImport("shcore.dll")]
        public static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
    }
}
