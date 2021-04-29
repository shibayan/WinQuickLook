using System.IO;
using System.Windows;
using System.Windows.Media;

namespace WinQuickLook.Controls
{
    public partial class GeneficFileViewer
    {
        public GeneficFileViewer()
        {
            InitializeComponent();
        }

        public ImageSource Thumbnail
        {
            get => (ImageSource)GetValue(ThumbnailProperty);
            set => SetValue(ThumbnailProperty, value);
        }

        public static readonly DependencyProperty ThumbnailProperty =
            DependencyProperty.Register("Thumbnail", typeof(ImageSource), typeof(GeneficFileViewer), new PropertyMetadata(null));

        public FileSystemInfo FileInfo
        {
            get => (FileInfo)GetValue(FileInfoProperty);
            set => SetValue(FileInfoProperty, value);
        }

        public static readonly DependencyProperty FileInfoProperty =
            DependencyProperty.Register("FileInfo", typeof(FileSystemInfo), typeof(GeneficFileViewer), new PropertyMetadata(null));
    }
}
