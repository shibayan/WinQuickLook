using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

using WinQuickLook.Extensions;
using WinQuickLook.Handlers;
using WinQuickLook.Internal;
using WinQuickLook.Interop;

namespace WinQuickLook
{
    public partial class QuickLookWindow
    {
        public QuickLookWindow()
        {
            InitializeComponent();
        }

        private string _fileName;

        private readonly IQuickLookHandler[] _fileHandlers =
        {
            new HtmlQuickLookHandler(),
            new SyntaxHighlightQuickLookHandler(),
            new TextQuickLookHandler(),
            new InternetShortcutQuickLookHandler(),
            new PdfQuickLookHandler(),
            new VideoQuickLookHandler(),
            new AudioQuickLookHandler(),
            new AnimatedGifQuickLookHandler(),
            new ImageQuickLookHandler(),
            new ComInteropQuickLookHandler()
        };

        public FrameworkElement PreviewHost
        {
            get => (FrameworkElement)GetValue(PreviewHostProperty);
            set => SetValue(PreviewHostProperty, value);
        }

        public static readonly DependencyProperty PreviewHostProperty =
            DependencyProperty.Register(nameof(PreviewHost), typeof(FrameworkElement), typeof(QuickLookWindow), new PropertyMetadata(null));

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            InitializeWindowStyle();
        }

        public bool HideIfVisible()
        {
            if (!IsVisible)
            {
                return false;
            }

            Hide();
            CleanupHost();

            return true;
        }

        public async void Open(string fileName)
        {
            CleanupHost();

            _fileName = fileName;

            var handler = default(IQuickLookHandler);

            if (File.Exists(fileName))
            {
                handler = _fileHandlers.FirstOrDefault(x => x.CanOpen(fileName));
            }

            var (element, requestSize, metadata) = await handler.GetViewerWithErrorAsync(fileName);

            PreviewHost = element;

            Title = $"{Path.GetFileName(fileName)}{(metadata == null ? "" : $" ({metadata})")}";

            SetAssociatedAppName(fileName);
            MoveWindowCentering(requestSize);

            Topmost = true;

            Show();

            Topmost = false;
        }

        private void CleanupHost()
        {
            if (PreviewHost is WindowsFormsHost formsHost)
            {
                formsHost.Child.Dispose();
                formsHost.Child = null;

                formsHost.Dispose();
            }

            PreviewHost = null;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (PreviewHost is Image image && image.StretchDirection != StretchDirection.Both)
            {
                image.StretchDirection = StretchDirection.Both;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanupHost();
        }


        private void OpenWithListButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OpenWithButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(_fileName) { UseShellExecute = true });

                HideIfVisible();
            }
            catch
            {
                MessageBox.Show(Strings.Resources.OpenButtonErrorMessage, "WinQuickLook");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HideIfVisible();
        }

        private void SetAssociatedAppName(string fileName)
        {
            var assocName = WinExplorerHelper.GetAssocName(fileName);

            if (string.IsNullOrEmpty(assocName))
            {
                openWithButton.Visibility = Visibility.Collapsed;
                //openWithListButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ((TextBlock)openWithButton.Content).Text = string.Format(Strings.Resources.OpenButtonText, assocName);

                openWithButton.Visibility = Visibility.Visible;
                //openWithListButton.Visibility = Visibility.Visible;
            }
        }

        private void InitializeWindowStyle()
        {
            var theme = PlatformHelper.GetWindowsTheme();

            Foreground = theme == WindowsTheme.Light ? Brushes.Black : Brushes.LightGray;

            var accentPolicy = new ACCENTPOLICY
            {
                nAccentState = 3,
                nFlags = 2,
                nColor = theme == WindowsTheme.Light ? 0xC0FFFFFF : 0xC0000000
            };

            var accentPolicySize = Marshal.SizeOf(accentPolicy);
            var accentPolicyPtr = Marshal.AllocHGlobal(accentPolicySize);

            Marshal.StructureToPtr(accentPolicy, accentPolicyPtr, false);

            var winCompatData = new WINCOMPATTRDATA
            {
                nAttribute = 19,
                ulDataSize = accentPolicySize,
                pData = accentPolicyPtr
            };

            var hwnd = new WindowInteropHelper(this).Handle;

            NativeMethods.SetWindowCompositionAttribute(hwnd, ref winCompatData);

            Marshal.FreeHGlobal(accentPolicyPtr);

            var style = NativeMethods.GetWindowLong(hwnd, Consts.GWL_STYLE);
            NativeMethods.SetWindowLong(hwnd, Consts.GWL_STYLE, style & ~(Consts.WS_SYSMENU | Consts.WS_MINIMIZEBOX | Consts.WS_MAXIMIZEBOX));

            var hwndSource = HwndSource.FromHwnd(hwnd);
            hwndSource.AddHook(WndProc);

            DisableSystemMenu();
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Consts.WM_SYSKEYDOWN:
                    if (wParam.ToInt32() == Consts.VK_F4)
                    {
                        handled = true;
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        private void MoveWindowCentering(Size requestSize)
        {
            var foregroundHwnd = NativeMethods.GetForegroundWindow();

            var hMonitor = NativeMethods.MonitorFromWindow(foregroundHwnd, Consts.MONITOR_DEFAULTTOPRIMARY);

            var monitorInfo = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };

            NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);

            var monitor = new Rect(monitorInfo.rcMonitor.x, monitorInfo.rcMonitor.y,
                monitorInfo.rcMonitor.cx - monitorInfo.rcMonitor.x, monitorInfo.rcMonitor.cy - monitorInfo.rcMonitor.y);

            NativeMethods.GetDpiForMonitor(hMonitor, Consts.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);

            var dpiFactorX = dpiX / 96.0;
            var dpiFactorY = dpiY / 96.0;

            var minWidthOrHeight = Math.Min(monitor.Width, monitor.Height) * 0.8;
            var scaleFactor = Math.Min(minWidthOrHeight / Math.Max(requestSize.Width, requestSize.Height), 1.0);

            Width = Math.Max(Math.Round(requestSize.Width * scaleFactor) + 10, MinWidth);
            Height = Math.Max(Math.Round(requestSize.Height * scaleFactor) + 40 + 5, MinHeight);

            var x = monitor.X + ((monitor.Width - (Width * dpiFactorX)) / 2);
            var y = monitor.Y + ((monitor.Height - (Height * dpiFactorY)) / 2);

            var hwnd = new WindowInteropHelper(this).Handle;

            NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, (int)Math.Round(x), (int)Math.Round(y), 0, 0, Consts.SWP_NOACTIVATE | Consts.SWP_NOSIZE | Consts.SWP_NOZORDER);
        }

        private void DisableSystemMenu()
        {
            var type = typeof(WindowChrome).Assembly.GetType("System.Windows.Shell.WindowChromeWorker");
            var method = type.GetMethod("GetWindowChromeWorker", BindingFlags.Public | BindingFlags.Static);
            var field = type.GetField("_messageTable", BindingFlags.Instance | BindingFlags.NonPublic);

            var windowChromeWorker = (DependencyObject)method.Invoke(null, new object[] { this });
            var messageTable = (IList)field.GetValue(windowChromeWorker);

            messageTable.RemoveAt(5);
        }
    }
}
