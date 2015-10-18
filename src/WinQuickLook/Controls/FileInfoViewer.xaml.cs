using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WinQuickLook.Controls
{
    public partial class FileInfoViewer
    {
        public FileInfoViewer()
        {
            InitializeComponent();
        }
        
        public BitmapSource Image
        {
            get { return (BitmapSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapSource), typeof(FileInfoViewer), new PropertyMetadata(null));
        
        public FileSystemInfo FileInfo
        {
            get { return (FileInfo)GetValue(FileInfoProperty); }
            set { SetValue(FileInfoProperty, value); }
        }

        public static readonly DependencyProperty FileInfoProperty =
            DependencyProperty.Register("FileInfo", typeof(FileSystemInfo), typeof(FileInfoViewer), new PropertyMetadata(null));
        
    }
}
