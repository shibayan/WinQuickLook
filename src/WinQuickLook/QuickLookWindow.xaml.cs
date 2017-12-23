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

namespace WinQuickLook
{
    public partial class QuickLookWindow
    {
        public QuickLookWindow()
        {
            InitializeComponent();

            open.Click += (sender, e) =>
            {
                Process.Start(_fileInfo.FullName);

                Close();
            };
        }

        private FileInfo _fileInfo;
        private IQuickLookPreviewHandler _handler;
        private bool _isClosed;

        private static readonly IQuickLookPreviewHandler[] _handlers =
        {
            new ImagePreviewHandler(),
            new VideoPreviewHandler(),
            new AudioPreviewHandler(),
            new HtmlPreviewHandler(),
            new SyntaxHighlightPreviewHandler(),
            new TextPreviewHandler(),
            new ComInteropPreviewHandler(),
            new GenericPreviewHandler()
        };

        public FrameworkElement PreviewHost
        {
            get => (FrameworkElement)GetValue(PreviewHostProperty);
            set => SetValue(PreviewHostProperty, value);
        }

        public static readonly DependencyProperty PreviewHostProperty =
            DependencyProperty.Register("PreviewHost", typeof(FrameworkElement), typeof(QuickLookWindow), new PropertyMetadata(null));

        public new void Close()
        {
            if (!_isClosed)
            {
                _isClosed = true;

                base.Close();
            }
        }

        public bool CloseIfActive()
        {
            if (!IsActive)
            {
                return false;
            }

            Close();

            return true;
        }

        public void Open(string fileName)
        {
            _handler = _handlers.First(x => x.CanOpen(fileName));

            var element = _handler.GetElement(fileName);

            PreviewHost = element;

            if (!double.IsNaN(element.Width) && !double.IsNaN(element.Height))
            {
                Width = Math.Max(element.Width + 4 + 2 + 2, MinWidth);
                Height = Math.Max(element.Height + 30 + 4 + 2 + 2, MinHeight);

                element.Width = double.NaN;
                element.Height = double.NaN;
            }

            _fileInfo = new FileInfo(fileName);
        }

        public new void Show()
        {
            if (PreviewHost is Image)
            {
                var bitmap = (BitmapSource)((Image)PreviewHost).Source;

                Title = $"{_fileInfo.Name} ({bitmap.PixelWidth}x{bitmap.PixelHeight} - {WinExplorerHelper.GetSizeFormat(_fileInfo.Length)})";
            }
            else
            {
                Title = _fileInfo.Name;
            }

            base.Show();

            Topmost = false;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            if (_handler.AllowsTransparency)
            {
                SetBlurEffect();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if (PreviewHost is Image image && image.StretchDirection != StretchDirection.Both)
                {
                    image.StretchDirection = StretchDirection.Both;
                }
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            (PreviewHost as WindowsFormsHost)?.Child.Dispose();
            PreviewHost = null;
        }

        private void SetBlurEffect()
        {
            Background = new SolidColorBrush(Color.FromArgb(0x90, 0xFF, 0xFF, 0xFF));
            WindowStyle = WindowStyle.None;

            var interopHelper = new WindowInteropHelper(this);

            var accentPolicy = new ACCENTPOLICY
            {
                nAccentState = 3
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

            NativeMethods.SetWindowCompositionAttribute(interopHelper.Handle, ref winCompatData);

            Marshal.FreeHGlobal(accentPolicyPtr);
        }
    }
}
