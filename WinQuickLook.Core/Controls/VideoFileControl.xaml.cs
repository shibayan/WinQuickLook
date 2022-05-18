using System;
using System.IO;

namespace WinQuickLook.Controls;

public partial class VideoFileControl
{
    public VideoFileControl()
    {
        InitializeComponent();
    }

    public void Open(FileInfo fileInfo)
    {
        mediaElement.Source = new Uri(fileInfo.FullName);
        mediaElement.Play();
    }
}
