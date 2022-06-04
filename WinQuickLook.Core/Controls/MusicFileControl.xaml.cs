using System.IO;
using System.Windows;

namespace WinQuickLook.Controls;

public partial class MusicFileControl
{
    public MusicFileControl()
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
        DependencyProperty.Register(nameof(FileInfo), typeof(FileInfo), typeof(MusicFileControl), new PropertyMetadata(null));

    public void Open(FileInfo fileInfo)
    {
        FileInfo = fileInfo;

        var musicProperties = new Shell.PropertyStore().GetMusicProperties(fileInfo);

        title.Text = musicProperties?.Title;
        artist.Text = musicProperties?.Artist;
        album.Text = musicProperties?.Album;

        mediaElement.Play();
    }
}
