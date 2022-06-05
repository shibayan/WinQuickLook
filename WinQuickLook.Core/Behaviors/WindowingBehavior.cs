using System;
using System.Windows;
using System.Windows.Interop;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace WinQuickLook.Behaviors;

public class WindowingBehavior
{
    public static bool GetPreventClosing(DependencyObject obj) => (bool)obj.GetValue(PreventClosingProperty);

    public static void SetPreventClosing(DependencyObject obj, bool value) => obj.SetValue(PreventClosingProperty, value);

    public static readonly DependencyProperty PreventClosingProperty =
        DependencyProperty.RegisterAttached("PreventClosing", typeof(bool), typeof(Window), new PropertyMetadata(false, PreventClosing_PropertyChangedCallback));

    public static bool GetSystemBackdrop(DependencyObject obj) => (bool)obj.GetValue(SystemBackdropProperty);

    public static void SetSystemBackdrop(DependencyObject obj, bool value) => obj.SetValue(SystemBackdropProperty, value);

    public static readonly DependencyProperty SystemBackdropProperty =
        DependencyProperty.RegisterAttached("SystemBackdrop", typeof(bool), typeof(Window), new PropertyMetadata(false, SystemBackdrop_PropertyChangedCallback));

    private static void SystemBackdrop_PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        var window = (Window)obj;

        if ((bool)e.NewValue)
        {
            window.SourceInitialized += SourceInitialized;
        }
        else
        {
            window.SourceInitialized -= SourceInitialized;
        }

        static void SourceInitialized(object? sender, EventArgs e)
        {
            var hwnd = new HWND(new WindowInteropHelper((Window)sender!).Handle);
            var trueValue = 0x01;

            PInvoke.DwmSetWindowAttribute(hwnd, (Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE)1029, ref trueValue);
        }
    }

    private static void PreventClosing_PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        var window = (Window)obj;

        if ((bool)e.NewValue)
        {
            window.SourceInitialized += SourceInitialized;
            window.Loaded += Loaded;
        }
        else
        {
            window.SourceInitialized -= SourceInitialized;
            window.Loaded -= Loaded;
        }

        static void SourceInitialized(object? sender, EventArgs e)
        {
            var hwnd = new HWND(new WindowInteropHelper((Window)sender!).Handle);

            var style = PInvoke.GetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            PInvoke.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style & ~(int)(WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_MAXIMIZEBOX));
        }

        static void Loaded(object sender, RoutedEventArgs e)
        {
            var hwndSource = HwndSource.FromHwnd(new WindowInteropHelper((Window)sender).Handle);

            hwndSource?.AddHook(WndProc);
        }
    }

    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch ((uint)msg)
        {
            case PInvoke.WM_SYSKEYDOWN when (VIRTUAL_KEY)wParam == VIRTUAL_KEY.VK_F4:
                handled = true;
                break;
            case PInvoke.WM_NCRBUTTONUP when (uint)wParam == PInvoke.HTCAPTION:
                handled = true;
                break;
        }

        return IntPtr.Zero;
    }
}
