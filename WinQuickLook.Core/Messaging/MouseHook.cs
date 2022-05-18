using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace WinQuickLook.Messaging;

public class MouseHook : WindowsHook
{
    public MouseHook()
        : base(WINDOWS_HOOK_ID.WH_MOUSE_LL)
    {
    }

    protected override LRESULT HookProc(int code, WPARAM wParam, LPARAM lParam)
    {
        if (code == PInvoke.HC_ACTION && wParam == (nuint)PInvoke.WM_LBUTTONDOWN)
        {

        }

        return base.HookProc(code, wParam, lParam);
    }
}
