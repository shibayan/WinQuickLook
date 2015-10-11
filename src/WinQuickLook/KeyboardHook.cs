using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WinQuickLook.Interop;

namespace WinQuickLook
{
    public class KeyboardHook : IDisposable
    {
        public KeyboardHook(Action performAction, Action cancelAction)
        {
            _performAction = performAction;
            _cancelAction = cancelAction;
        }

        private IntPtr _hook;

        private readonly Action _performAction;
        private readonly Action _cancelAction;

        private NativeMethods.LowLevelKeyboardProc _keyboardHookProc;
        
        public void Start()
        {
            if (_hook != IntPtr.Zero)
            {
                return;
            }

            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                _keyboardHookProc = KeyboardHookProc;

                _hook = NativeMethods.SetWindowsHookEx(Consts.WH_KEYBOARD_LL, _keyboardHookProc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
            }
        }

        public void Dispose()
        {
            if (_hook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hook);

                _hook = IntPtr.Zero;
            }
        }

        private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)Consts.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode == (int)Keys.Space)
                {
                    _performAction();
                }
                else if (vkCode == (int)Keys.Escape)
                {
                    _cancelAction();
                }
            }

            return NativeMethods.CallNextHookEx(_hook, nCode, wParam, lParam);
        }
    }
}
