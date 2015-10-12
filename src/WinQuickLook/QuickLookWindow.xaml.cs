using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms.Integration;

using WinQuickLook.Handlers;

namespace WinQuickLook
{
    public partial class QuickLookWindow
    {
        public QuickLookWindow()
        {
            InitializeComponent();

            DataContext = this;

            Deactivated += (sender, e) => Close();

            open.Click += (sender, e) =>
            {
                Process.Start(_fileInfo.FullName);
            };
        }

        private FileInfo _fileInfo;

        private readonly IQuickLookHandler[] _handlers =
        {
            new ImagePreviewHandler(),
            new TextPreviewHandler(),
            new ComInteropPreviewHandler(),
            new ShellPreviewHandler(),
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

            PreviewHost = handler.GetElement(fileName);

            _fileInfo = new FileInfo(fileName);
        }

        public new void Show()
        {
            Title = _fileInfo.Name;

            base.Show();

            Dispatcher.InvokeAsync(() => Activate());
        }
    }
}
