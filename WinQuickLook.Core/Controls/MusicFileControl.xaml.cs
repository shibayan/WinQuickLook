using System;
using System.IO;
using System.Windows.Media;

namespace WinQuickLook.Controls;

public partial class MusicFileControl
{
    public MusicFileControl()
    {
        InitializeComponent();
    }

    private readonly MediaPlayer _mediaPlayer = new();

    public void Open(FileInfo fileInfo)
    {
        var musicProperties = new Shell.PropertyStore().GetMusicProperties(fileInfo);
        var image = new Shell.PreviewImageFactory().GetImage(fileInfo);

        thumbnail.Source = image;
        title.Text = musicProperties?.Title;

        _mediaPlayer.Open(new Uri(fileInfo.FullName));
        _mediaPlayer.Play();
    }
}
