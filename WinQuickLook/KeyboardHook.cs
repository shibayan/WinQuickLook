using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;

using WinQuickLook.Interop;

namespace WinQuickLook
{
    public class KeyboardHook : IDisposable
    {
        public KeyboardHook(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            _keyboardHookProc = KeyboardHookProc;
        }

        public Dispatcher Dispatcher { get; }

        public Action PerformAction { get; set; }
        public Action ChangeAction { get; set; }
        public Action CancelAction { get; set; }

        private IntPtr _hook;
        private readonly NativeMethods.LowLevelKeyboardProc _keyboardHookProc;

        public void Start()
        {
            if (_hook != IntPtr.Zero)
            {
                return;
            }

            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                _hook = NativeMethods.SetWindowsHookEx(Consts.WH_KEYBOARD_LL, _keyboardHookProc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
            }
        }

        public void Stop()
        {
            if (_hook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hook);

                _hook = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == Consts.HC_ACTION && wParam == (IntPtr)Consts.WM_KEYDOWN && IsNotModifierKeys())
            {
                var kbdllhook = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                switch ((Keys)kbdllhook.vkCode)
                {
                    case Keys.Space:
                        Dispatcher.InvokeAsync(PerformAction);
                        break;

                    case Keys.Escape:
                        Dispatcher.InvokeAsync(CancelAction);
                        break;

                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        Dispatcher.InvokeAsync(ChangeAction);
                        break;
                }
            }

            return NativeMethods.CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        private static bool IsNotModifierKeys()
        {
            if (NativeMethods.GetAsyncKeyState((int)Keys.ControlKey) != 0)
            {
                return false;
            }

            if (NativeMethods.GetAsyncKeyState((int)Keys.Menu) != 0)
            {
                return false;
            }

            if (NativeMethods.GetAsyncKeyState((int)Keys.ShiftKey) != 0)
            {
                return false;
            }

            if (NativeMethods.GetAsyncKeyState((int)Keys.LWin) != 0 || NativeMethods.GetAsyncKeyState((int)Keys.RWin) != 0)
            {
                return false;
            }

            return true;
        }
    }
}
