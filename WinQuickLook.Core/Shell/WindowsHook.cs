using System;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace WinQuickLook.Shell;

public abstract class WindowsHook : IDisposable
{
    protected WindowsHook(WINDOWS_HOOK_ID idHook)
    {
        _idHook = idHook;
    }

    private readonly WINDOWS_HOOK_ID _idHook;

    private HHOOK _hook;
    private bool _disposed;

    public void Start()
    {
        if (!_hook.IsNull)
        {
            return;
        }

        PInvoke.SetWindowsHookEx(_idHook, HookProc, PInvoke.GetModuleHandle(new PCWSTR()), 0);
    }

    public void Stop()
    {
        if (_hook.IsNull)
        {
            return;
        }

        PInvoke.UnhookWindowsHookEx(_hook);

        _hook = new HHOOK();
    }

    protected virtual LRESULT HookProc(int code, WPARAM wParam, LPARAM lParam) => PInvoke.CallNextHookEx(_hook, code, wParam, lParam);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            Stop();

            _disposed = true;
        }
    }

    ~WindowsHook() => Dispose(false);
}
