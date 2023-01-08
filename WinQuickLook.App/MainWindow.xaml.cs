using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Cylinder;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.WindowsAndMessaging;

using WinQuickLook.Extensions;
using WinQuickLook.Handlers;
using WinQuickLook.Windows;

namespace WinQuickLook.App;

public partial class MainWindow
{
    public MainWindow(IEnumerable<IFileSystemPreviewHandler> previewHandlers, AssociationResolver associationResolver)
    {
        InitializeComponent();

        _previewHandlers = previewHandlers;
        _associationResolver = associationResolver;
    }

    private readonly IEnumerable<IFileSystemPreviewHandler> _previewHandlers;
    private readonly AssociationResolver _associationResolver;

    public Ref<FileInfo> FileInfo { get; } = new(null);

    public Ref<string> DefaultName { get; } = new("");

    public Ref<IReadOnlyList<AssociationResolver.Entry>> Recommends { get; } = new(Array.Empty<AssociationResolver.Entry>());

    public void OpenPreview(FileSystemInfo fileSystemInfo)
    {
        if (!_previewHandlers.TryCreateViewer(fileSystemInfo, out var handlerResult))
        {
            return;
        }

        ApplyRequestSize(handlerResult.RequestSize);

        Title = fileSystemInfo.Name;
        contentPresenter.Content = handlerResult.Viewer;

        if (fileSystemInfo is FileInfo fileInfo)
        {
            FileInfo.Value = fileInfo;

            DefaultName.Value = _associationResolver.TryGetDefault(fileInfo, out var entry) ? entry.Name : "";
            Recommends.Value = _associationResolver.GetRecommends(fileInfo);
        }
        else
        {
            DefaultName.Value = "";
            Recommends.Value = Array.Empty<AssociationResolver.Entry>();
        }

        if (IsVisible)
        {
            return;
        }

        MoveCenter();
        Show();
    }

    public void OpenWithAssociation(string appName)
    {
        if (FileInfo.Value is null)
        {
            return;
        }

        _associationResolver.Invoke(appName, FileInfo.Value);

        ClosePreview();
    }

    public void OpenWithDefault()
    {
        if (FileInfo.Value is null)
        {
            return;
        }

        Process.Start(new ProcessStartInfo(FileInfo.Value.FullName) { UseShellExecute = true });

        ClosePreview();
    }

    public void ClosePreview()
    {
        Hide();

        contentPresenter.Content = null;
    }

    public void ClosePreviewIfActive()
    {
        if (IsActive)
        {
            ClosePreview();
        }
    }

    private void ApplyRequestSize(Size requestSize)
    {
        var (monitor, _) = GetCurrentMonitorInfo();

        var minWidthOrHeight = Math.Min(monitor.Width, monitor.Height) * 0.8;
        var scaleFactor = Math.Min(minWidthOrHeight / Math.Max(requestSize.Width, requestSize.Height), 1.0);

        Width = Math.Max(Math.Round(requestSize.Width * scaleFactor), MinWidth);
        Height = Math.Max(Math.Round(requestSize.Height * scaleFactor) + AppParameters.CaptionHeight, MinHeight);
    }

    private void MoveCenter()
    {
        var (monitor, dpi) = GetCurrentMonitorInfo();

        var dpiFactor = dpi / 96.0;

        var x = monitor.X + ((monitor.Width - (Width * dpiFactor)) / 2);
        var y = monitor.Y + ((monitor.Height - (Height * dpiFactor)) / 2);

        var hwnd = new WindowInteropHelper(this).Handle;

        PInvoke.SetWindowPos(new HWND(hwnd), new HWND(), (int)Math.Round(x), (int)Math.Round(y), 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
    }

    private static (Rect, double) GetCurrentMonitorInfo()
    {
        var foregroundHwnd = PInvoke.GetForegroundWindow();

        var hMonitor = PInvoke.MonitorFromWindow(foregroundHwnd, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);

        var monitorInfo = new MONITORINFO
        {
            cbSize = (uint)Marshal.SizeOf<MONITORINFO>()
        };

        PInvoke.GetMonitorInfo(hMonitor, ref monitorInfo);
        PInvoke.GetDpiForMonitor(hMonitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out _);

        var leftTop = new Point(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top);
        var rightBottom = new Point(monitorInfo.rcMonitor.right, monitorInfo.rcMonitor.bottom);

        return (new Rect(leftTop, rightBottom), dpiX);
    }
}
