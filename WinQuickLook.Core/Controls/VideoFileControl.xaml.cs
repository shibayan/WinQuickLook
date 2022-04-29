using System;
using System.IO;
using System.Windows.Controls;

namespace WinQuickLook.Controls;

public partial class VideoFileControl : UserControl
{
    public VideoFileControl()
    {
        InitializeComponent();
    }

    public void Open(FileInfo fileInfo)
    {
        mediaElement.Source = new Uri(fileInfo.FullName);
    }
}
