using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;

using WinQuickLook.Interop;

namespace WinQuickLook
{
    public class MessageHook : IDisposable
    {
        public MessageHook(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            _keyboardHookProc = KeyboardHookProc;
            _mouseHookProc = MouseHookProc;
        }

        public Dispatcher Dispatcher { get; }

        public Action PerformAction { get; set; }
        public Action ChangeAction { get; set; }
        public Action CancelAction { get; set; }

        private IntPtr _keyboardHook;
        private readonly NativeMethods.LowLevelHookProc _keyboardHookProc;

        private IntPtr _mouseHook;
        private readonly NativeMethods.LowLevelHookProc _mouseHookProc;

        public void Start()
        {
            StartKeyboardHook();

#if !DEBUG
            StartMouseHook();
#endif
        }

        public void Stop()
        {
            StopKeyboardHook();
            StopMouseHook();
        }

        public void Dispose()
        {
            Stop();
        }

        private void StartKeyboardHook()
        {
            if (_keyboardHook != IntPtr.Zero)
            {
                return;
            }

            using var process = Process.GetCurrentProcess();
            using var module = process.MainModule;

            _keyboardHook = NativeMethods.SetWindowsHookEx(Consts.WH_KEYBOARD_LL, _keyboardHookProc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
        }

        private void StopKeyboardHook()
        {
            if (_keyboardHook == IntPtr.Zero)
            {
                return;
            }

            NativeMethods.UnhookWindowsHookEx(_keyboardHook);

            _keyboardHook = IntPtr.Zero;
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

            return NativeMethods.CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        private void StartMouseHook()
        {
            if (_mouseHook != IntPtr.Zero)
            {
                return;
            }

            using var process = Process.GetCurrentProcess();
            using var module = process.MainModule;

            _mouseHook = NativeMethods.SetWindowsHookEx(Consts.WH_MOUSE_LL, _mouseHookProc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
        }

        private void StopMouseHook()
        {
            if (_mouseHook == IntPtr.Zero)
            {
                return;
            }

            NativeMethods.UnhookWindowsHookEx(_mouseHook);

            _mouseHook = IntPtr.Zero;
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == Consts.HC_ACTION && wParam == (IntPtr)Consts.WM_LBUTTONDOWN)
            {
                Dispatcher.InvokeAsync(ChangeAction);
            }

            return NativeMethods.CallNextHookEx(_mouseHook, nCode, wParam, lParam);
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
