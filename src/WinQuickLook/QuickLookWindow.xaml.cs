using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

using WinQuickLook.Handlers;

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

        private readonly IQuickLookHandler[] _handlers =
        {
            new ImagePreviewHandler(),
            new VideoPreviewHandler(),
            new HtmlPreviewHandler(),
            new TextPreviewHandler(),
            new ComInteropPreviewHandler(),
            new ShellImagePreviewHandler(),
        };

        public bool IsClosed { get; set; }
        
        public UIElement PreviewHost
        {
            get { return (UIElement)GetValue(PreviewHostProperty); }
            set { SetValue(PreviewHostProperty, value); }
        }
        
        public static readonly DependencyProperty PreviewHostProperty =
            DependencyProperty.Register("PreviewHost", typeof(UIElement), typeof(QuickLookWindow), new PropertyMetadata(null));

        public new void Close()
        {
            if (!IsClosed)
            {
                IsClosed = true;

                base.Close();
                
                (PreviewHost as WindowsFormsHost)?.Child.Dispose();
                PreviewHost = null;
            }
        }

        public void Open(string fileName)
        {
            var handler = _handlers.First(x => x.CanOpen(fileName));

            var element = handler.GetElement(fileName);

            PreviewHost = element;

            if (!double.IsNaN(element.Width) && !double.IsNaN(element.Height))
            {
                Width = Math.Max(element.Width + 4 + 2 + 2, 300);
                Height = Math.Max(element.Height + 30 + 4 + 2 + 2, 200);

                element.Width = double.NaN;
                element.Height = double.NaN;
            }

            _fileInfo = new FileInfo(fileName);
        }

        public new void Show()
        {
            Title = _fileInfo.Name;

            base.Show();

            Dispatcher.InvokeAsync(() => Activate());
        }
        
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (IsLoaded)
            {
                var image = PreviewHost as Image;

                if (image != null && image.StretchDirection != StretchDirection.Both)
                {
                    image.StretchDirection = StretchDirection.Both;
                }

                var mediaElement = PreviewHost as MediaElement;

                if (mediaElement != null && mediaElement.StretchDirection != StretchDirection.Both)
                {
                    mediaElement.StretchDirection = StretchDirection.Both;
                }
            }
        }
    }
}
