using System.IO;
using System.Windows;

namespace WinQuickLook.Controls;

public partial class AudioFileControl
{
    public AudioFileControl()
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
        DependencyProperty.Register(nameof(FileInfo), typeof(FileInfo), typeof(AudioFileControl), new PropertyMetadata(null));

    public void Open(FileInfo fileInfo)
    {
        FileInfo = fileInfo;

        var audioProperties = new Shell.PropertyStore().GetAudioProperties(fileInfo);

        title.Text = audioProperties?.Title;
        artist.Text = audioProperties?.Artist;
        album.Text = audioProperties?.Album;

        mediaElement.Play();
    }
}
