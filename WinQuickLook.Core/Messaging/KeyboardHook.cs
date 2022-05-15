using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace WinQuickLook.Messaging;

public class KeyboardHook : WindowsHook
{
    public KeyboardHook(Dispatcher dispatcher)
        : base(WINDOWS_HOOK_ID.WH_KEYBOARD_LL)
    {
        _dispatcher = dispatcher;
    }

    private readonly Dispatcher _dispatcher;

    public Action? PerformSpaceKey { get; set; }

    protected override LRESULT HookProc(int code, WPARAM wParam, LPARAM lParam)
    {
        if (code == PInvoke.HC_ACTION && wParam == (nuint)PInvoke.WM_KEYDOWN)
        {
            var kbdllhook = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

            switch ((VIRTUAL_KEY)kbdllhook.vkCode)
            {
                case VIRTUAL_KEY.VK_SPACE:
                    _dispatcher.InvokeAsync(() =>
                    {
                        PerformSpaceKey?.Invoke();
                    });

                    break;
            }
        }

        return base.HookProc(code, wParam, lParam);
    }
}
