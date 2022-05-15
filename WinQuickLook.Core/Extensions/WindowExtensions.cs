using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;

namespace WinQuickLook.Extensions;

public static class WindowExtensions
{
    public static void Preload(this Window window)
    {
        var minWidth = window.MinWidth;
        var minHeight = window.MinHeight;

        window.MinWidth = 0;
        window.MinHeight = 0;
        window.Width = 0;
        window.Height = 0;
        window.Left = -65535;
        window.Top = -65535;
        window.Show();
        window.Hide();

        window.MinWidth = minWidth;
        window.MinHeight = minHeight;
    }
}
