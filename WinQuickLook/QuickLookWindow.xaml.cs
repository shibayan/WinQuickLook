using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Handlers;
using WinQuickLook.Interop;

using IPreviewHandler = WinQuickLook.Handlers.IPreviewHandler;

namespace WinQuickLook
{
    public partial class QuickLookWindow
    {
        public QuickLookWindow()
        {
            InitializeComponent();
        }

        private FileInfo _fileInfo;
        private IPreviewHandler _handler;

        private static readonly IPreviewHandler[] _handlers =
        {
            new SyntaxHighlightPreviewHandler(),
            new TextPreviewHandler(),
            new HtmlPreviewHandler(),
            new InternetShortcutPreviewHandler(),
            new PdfPreviewHandler(),
            new VideoPreviewHandler(),
            new AudioPreviewHandler(),
            new AnimatedGifPreviewHandler(),
            new ImagePreviewHandler(),
            new ComInteropPreviewHandler(),
            new GenericPreviewHandler()
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

            SetBlurEffect();

            MoveWindowCentering();
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

        public void Open(string fileName)
        {
            CleanupHost();

            _handler = _handlers.First(x => x.CanOpen(fileName));

            var monitorSize = GetCurrentMonitorSize();

            var (element, requestSize) = _handler.GetViewer(fileName, monitorSize);

            PreviewHost = element;

            Width = Math.Max(requestSize.Width + 10, MinWidth);
            Height = Math.Max(requestSize.Height + 40 + 5, MinHeight);

            _fileInfo = new FileInfo(fileName);

            MoveWindowCentering();

            SetAssocName(fileName);
        }

        public new void Show()
        {
            if (PreviewHost is Image image)
            {
                var bitmap = (BitmapSource)image.Source;

                Title = $"{_fileInfo.Name} ({bitmap.PixelWidth}x{bitmap.PixelHeight} - {WinExplorerHelper.GetSizeFormat(_fileInfo.Length)})";
            }
            else
            {
                Title = _fileInfo.Name;
            }

            Topmost = true;

            base.Show();

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

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(_fileInfo.FullName) { UseShellExecute = true });

                HideIfVisible();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.OpenButtonErrorMessage, "WinQuickLook");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HideIfVisible();
        }

        private void SetBlurEffect()
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
            NativeMethods.SetWindowLong(hwnd, Consts.GWL_STYLE, style & ~Consts.WS_SYSMENU);
        }

        private void SetAssocName(string fileName)
        {
            var assocName = WinExplorerHelper.GetAssocName(fileName);

            if (string.IsNullOrEmpty(assocName))
            {
                open.Visibility = Visibility.Collapsed;
            }
            else
            {
                open.ToolTip = string.Format(Properties.Resources.OpenButtonText, assocName);
                open.Visibility = Visibility.Visible;
            }
        }

        private Size GetCurrentMonitorSize()
        {
            var foregroundHwnd = NativeMethods.GetForegroundWindow();

            var hMonitor = NativeMethods.MonitorFromWindow(foregroundHwnd, Consts.MONITOR_DEFAULTTOPRIMARY);

            var monitorInfo = new MONITORINFO
            {
                cbSize = Marshal.SizeOf<MONITORINFO>()
            };

            NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);

            return new Size(monitorInfo.rcMonitor.cx - monitorInfo.rcMonitor.x, monitorInfo.rcMonitor.cy - monitorInfo.rcMonitor.y);
        }

        private void MoveWindowCentering()
        {
            var foregroundHwnd = NativeMethods.GetForegroundWindow();

            var hMonitor = NativeMethods.MonitorFromWindow(foregroundHwnd, Consts.MONITOR_DEFAULTTOPRIMARY);

            var monitorInfo = new MONITORINFO
            {
                cbSize = Marshal.SizeOf<MONITORINFO>()
            };

            NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);

            var monitor = new Rect(monitorInfo.rcMonitor.x, monitorInfo.rcMonitor.y,
                monitorInfo.rcMonitor.cx - monitorInfo.rcMonitor.x, monitorInfo.rcMonitor.cy - monitorInfo.rcMonitor.y);

            NativeMethods.GetDpiForMonitor(hMonitor, Consts.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);

            var dpiFactorX = dpiX / 96.0;
            var dpiFactorY = dpiY / 96.0;

            var hwnd = new WindowInteropHelper(this).Handle;

            var x = monitor.X + ((monitor.Width - (Width * dpiFactorX)) / 2);
            var y = monitor.Y + ((monitor.Height - (Height * dpiFactorY)) / 2);

            NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, (int)Math.Round(x), (int)Math.Round(y), 0, 0, Consts.SWP_NOACTIVATE | Consts.SWP_NOSIZE | Consts.SWP_NOZORDER);
        }
    }
}
