using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

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
                Process.Start(FileInfo.FullName);
            };
        }

        public bool IsClosed { get; set; }

        public FileInfo FileInfo { get; set; }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(QuickLookWindow), new PropertyMetadata(null));

        public double ContentWidth
        {
            get { return (double)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register("ContentWidth", typeof(double), typeof(QuickLookWindow), new PropertyMetadata(0.0));

        public double ContentHeight
        {
            get { return (double)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }

        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register("ContentHeight", typeof(double), typeof(QuickLookWindow), new PropertyMetadata(0.0));

        public new void Close()
        {
            if (!IsClosed)
            {
                IsClosed = true;

                base.Close();

                Image = null;
                previewHandlerHost.Dispose();
            }
        }

        public new void Show()
        {
            Title = FileInfo.Name;

            base.Show();

            Dispatcher.InvokeAsync(() => Activate());
        }

        public void OpenPreview()
        {
            previewHandlerHost.Size = new System.Drawing.Size((int)ContentWidth, (int)ContentHeight);
            previewHandlerHost.Open(FileInfo.FullName);

            windowsFormsHost.Visibility = Visibility.Visible;
        }
    }
}
