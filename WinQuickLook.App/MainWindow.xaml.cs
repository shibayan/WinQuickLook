using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.WindowsAndMessaging;

using WinQuickLook.Extensions;
using WinQuickLook.Handlers;

namespace WinQuickLook.App;

public partial class MainWindow : Window
{
    public MainWindow(IEnumerable<IFileSystemPreviewHandler> previewHandlers)
    {
        InitializeComponent();

        _previewHandlers = previewHandlers;
    }

    private readonly IEnumerable<IFileSystemPreviewHandler> _previewHandlers;

    public void StartPreview(FileSystemInfo fileSystemInfo)
    {
        contentPresenter.Content = null;

        if (!_previewHandlers.TryCreateViewer(fileSystemInfo, out var handlerResult))
        {
            return;
        }

        ApplyRequestSize(handlerResult.RequestSize);

        contentPresenter.Content = handlerResult.Viewer;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        Application.Current.Shutdown();
    }

    private void ApplyRequestSize(Size requestSize)
    {
        var foregroundHwnd = PInvoke.GetForegroundWindow();

        var hMonitor = PInvoke.MonitorFromWindow(foregroundHwnd, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);

        var monitorInfo = new MONITORINFO
        {
            cbSize = (uint)Marshal.SizeOf<MONITORINFO>()
        };

        PInvoke.GetMonitorInfo(hMonitor, ref monitorInfo);

        var monitor = new Rect(new Point(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top), new Point(monitorInfo.rcMonitor.right, monitorInfo.rcMonitor.bottom));

        PInvoke.GetDpiForMonitor(hMonitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);

        var dpiXFactor = dpiX / 96.0;
        var dpiYFactor = dpiY / 96.0;

        var minWidthOrHeight = Math.Min(monitor.Width, monitor.Height) * 0.8;
        var scaleFactor = Math.Min(minWidthOrHeight / Math.Max(requestSize.Width, requestSize.Height), 1.0);

        Width = Math.Max(Math.Round(requestSize.Width * scaleFactor), MinWidth);
        Height = Math.Max(Math.Round(requestSize.Height * scaleFactor) + AppParameters.CaptionHeight, MinHeight);

        var x = monitor.X + ((monitor.Width - (Width * dpiXFactor)) / 2);
        var y = monitor.Y + ((monitor.Height - (Height * dpiYFactor)) / 2);

        var hwnd = new WindowInteropHelper(this).Handle;

        PInvoke.SetWindowPos(new HWND(hwnd), new HWND(), (int)Math.Round(x), (int)Math.Round(y), 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
    }
}
