using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Handlers;
using WinQuickLook.Interop;

namespace WinQuickLook
{
    public partial class QuickLookWindow
    {
        public QuickLookWindow()
        {
            InitializeComponent();
        }

        private FileInfo _fileInfo;
        private IQuickLookPreviewHandler _handler;

        private static readonly IQuickLookPreviewHandler[] _handlers =
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

            var element = _handler.GetElement(fileName);

            PreviewHost = element;

            var monitor = WinExplorerHelper.GetCurrentMonitor();

            if (!double.IsNaN(element.Width) && !double.IsNaN(element.Height))
            {
                Width = Math.Max(element.Width + 4 + 2 + 2 + 2, MinWidth);
                Height = Math.Max(element.Height + 40 + 4 + 2 + 2 + 2, MinHeight);

                element.Width = double.NaN;
                element.Height = double.NaN;
            }

            Left = monitor.X + ((monitor.Width - Width) / 2);
            Top = monitor.Y + ((monitor.Height - Height) / 2);

            _fileInfo = new FileInfo(fileName);

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

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            SetBlurEffect();
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
                Process.Start(_fileInfo.FullName);

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
            WindowStyle = WindowStyle.None;

            var theme = PlatformHelper.GetWindowsTheme();

            Foreground = theme == WindowsTheme.Light ? Brushes.Black : Brushes.White;

            var accentPolicy = new ACCENTPOLICY
            {
                nAccentState = 3,
                nFlags = 2,
                nColor = theme == WindowsTheme.Light ? 0x90FFFFFF : 0x90000000
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

            var interopHelper = new WindowInteropHelper(this);

            NativeMethods.SetWindowCompositionAttribute(interopHelper.Handle, ref winCompatData);

            Marshal.FreeHGlobal(accentPolicyPtr);
        }

        private void SetAssocName(string fileName)
        {
            int pcchOut = 0;

            NativeMethods.AssocQueryString(ASSOCF.INIT_IGNOREUNKNOWN, ASSOCSTR.FRIENDLYAPPNAME, Path.GetExtension(fileName), null, null, ref pcchOut);

            if (pcchOut == 0)
            {
                open.Visibility = Visibility.Collapsed;
                return;
            }

            var pszOut = new StringBuilder(pcchOut);

            NativeMethods.AssocQueryString(ASSOCF.INIT_IGNOREUNKNOWN, ASSOCSTR.FRIENDLYAPPNAME, Path.GetExtension(fileName), null, pszOut, ref pcchOut);

            open.ToolTip = string.Format(Properties.Resources.OpenButtonText, pszOut);
            open.Visibility = Visibility.Visible;
        }
    }
}
