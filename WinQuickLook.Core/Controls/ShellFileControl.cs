﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.PropertiesSystem;
using Windows.Win32.UI.WindowsAndMessaging;

using WinQuickLook.CsWin32;
using WinQuickLook.Extensions;

namespace WinQuickLook.Controls;

public class ShellFileControl : HwndHost
{
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        if (_previewHandler is not null)
        {
            var rect = new RECT { left = 0, top = 0, right = (int)sizeInfo.NewSize.Width, bottom = (int)sizeInfo.NewSize.Height };

            _previewHandler.SetRect(rect);
        }
    }

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        var hwndHost = PInvoke.CreateWindowEx(0,
            "static", "",
            WINDOW_STYLE.WS_CHILD | WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_CLIPCHILDREN,
            0, 0,
            (int)ActualWidth, (int)ActualHeight,
            new HWND(hwndParent.Handle));

        return new HandleRef(null, hwndHost);
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        if (_previewHandler is not null)
        {
            UnloadPreviewHandler(_previewHandler);

            _previewHandler = null;
        }

        PInvoke.DestroyWindow(new HWND(hwnd.Handle));
    }

    private IPreviewHandler? _previewHandler;

    public bool Open(FileInfo fileInfo)
    {
        if (!TryGetPreviewHandlerCLSID(fileInfo, out var clsid))
        {
            return false;
        }

        if (!TryCreatePreviewHandler(clsid, out var previewHandler))
        {
            return false;
        }

        if (!TryInitializePreviewHandler(previewHandler, fileInfo))
        {
            UnloadPreviewHandler(previewHandler);

            return false;
        }

        var rect = new RECT { left = 0, top = 0, right = (int)ActualWidth, bottom = (int)ActualHeight };

        previewHandler.SetWindow(new HWND(Handle), rect);
        previewHandler.DoPreview();

        if (_previewHandler is not null)
        {
            UnloadPreviewHandler(_previewHandler);
        }

        _previewHandler = previewHandler;

        return true;
    }

    // ReSharper disable once InconsistentNaming
    private static bool TryGetPreviewHandlerCLSID(FileInfo fileInfo, out Guid clsid)
    {
        var pcchOut = 40u;
        Span<char> pszOut = stackalloc char[(int)pcchOut];

        var riid = typeof(IPreviewHandler).GUID.ToString("B");

        if (PInvoke.AssocQueryString(0x00000004, ASSOCSTR.ASSOCSTR_SHELLEXTENSION, fileInfo.Extension, riid, pszOut, ref pcchOut).Value < 0)
        {
            clsid = Guid.Empty;

            return false;
        }

        clsid = Guid.Parse(pszOut[..(int)(pcchOut - 1)]);

        return true;
    }

    private static bool TryCreatePreviewHandler(Guid clsid, [NotNullWhen(true)] out IPreviewHandler? previewHandler)
    {
        if (PInvoke.CoCreateInstance(clsid, null, CLSCTX.CLSCTX_LOCAL_SERVER, out previewHandler).Value < 0)
        {
            return false;
        }

        return previewHandler is not null;
    }

    private static bool TryInitializePreviewHandler(IPreviewHandler previewHandler, FileInfo fileInfo)
    {
        switch (previewHandler)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            case IInitializeWithFile initializeWithFile:
                {
                    return initializeWithFile.Initialize(fileInfo.FullName, 0).Value >= 0;
                }
            // ReSharper disable once SuspiciousTypeConversion.Global
            case IInitializeWithItem initializeWithItem:
                {
                    if (PInvoke.SHCreateItemFromParsingName(fileInfo.FullName, null, out IShellItem shellItem).Value < 0)
                    {
                        return false;
                    }

                    try
                    {
                        return initializeWithItem.Initialize(shellItem, 0).Value >= 0;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(shellItem);
                    }
                }
            // ReSharper disable once SuspiciousTypeConversion.Global
            case IInitializeWithStream initializeWithStream:
                {
                    using var fileStream = fileInfo.OpenReadNoLock();

                    return initializeWithStream.Initialize(new StreamWrapper(fileStream), 0).Value >= 0;
                }
            default:
                return false;
        }
    }

    private static void UnloadPreviewHandler(IPreviewHandler previewHandler)
    {
        previewHandler.Unload();

        Marshal.FinalReleaseComObject(previewHandler);
    }
}
