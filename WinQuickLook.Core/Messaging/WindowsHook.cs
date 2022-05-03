﻿using System;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace WinQuickLook.Messaging;

public abstract class WindowsHook : IDisposable
{
    protected WindowsHook(WINDOWS_HOOK_ID idHook)
    {
        _idHook = idHook;
    }

    private readonly WINDOWS_HOOK_ID _idHook;

    private UnhookWindowsHookExSafeHandle? _hook;
    private bool _disposed;

    public void Start()
    {
        if (_hook is not null)
        {
            return;
        }

        _hook = PInvoke.SetWindowsHookEx(_idHook, HookProc, PInvoke.GetModuleHandle((string)null!), 0);
    }

    public void Stop()
    {
        if (_hook is null)
        {
            return;
        }

        _hook.Close();

        _hook = null;
    }

    protected virtual LRESULT HookProc(int code, WPARAM wParam, LPARAM lParam) => PInvoke.CallNextHookEx(_hook, code, wParam, lParam);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        Stop();

        _disposed = true;
    }

    ~WindowsHook() => Dispose(false);
}
