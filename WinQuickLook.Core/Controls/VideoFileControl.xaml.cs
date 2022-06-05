using System.IO;
using System.Windows;

namespace WinQuickLook.Controls;

public partial class VideoFileControl
{
    public VideoFileControl()
    {
        InitializeComponent();

        DataContext = this;
    }

    public FileInfo FileInfo
    {
        get => (FileInfo)GetValue(FileInfoProperty);
        set => SetValue(FileInfoProperty, value);
    }

    public static readonly DependencyProperty FileInfoProperty =
        DependencyProperty.Register(nameof(FileInfo), typeof(FileInfo), typeof(VideoFileControl), new PropertyMetadata(null));

    public void Open(FileInfo fileInfo)
    {
        FileInfo = fileInfo;

        mediaElement.Play();
    }
}
