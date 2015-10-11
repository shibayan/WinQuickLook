using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WinQuickLook.Interop;

namespace WinQuickLook
{
    public class KeyboardHook : IDisposable
    {
        public KeyboardHook(Action perform, Action cancel)
        {
            _perform = perform;
            _cancel = cancel;
        }

        private IntPtr _hook;

        private readonly Action _perform;
        private readonly Action _cancel;

        private NativeMethods.LowLevelKeyboardProc _keyboardHookProc;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        
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

                _hook = NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardHookProc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
            }
        }

        public void Dispose()
        {
            if (_hook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hook);
            }
        }

        private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode == (int)Keys.Space)
                {
                    _perform();
                }
                else if (vkCode == (int)Keys.Escape)
                {
                    _cancel();
                }
            }

            return NativeMethods.CallNextHookEx(_hook, nCode, wParam, lParam);
        }
    }
}
