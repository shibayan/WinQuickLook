﻿using System.IO;

using Cylinder;

using WinQuickLook.Providers;

namespace WinQuickLook.Controls;

public partial class AudioFileControl
{
    public AudioFileControl() => InitializeComponent();

    public Ref<FileInfo> FileInfo { get; } = new(null);

    public void Open(FileInfo fileInfo)
    {
        FileInfo.Value = fileInfo;

        var audioProperties = new ShellPropertyProvider().GetAudioProperties(fileInfo);

        title.Text = audioProperties?.Title;
        artist.Text = audioProperties?.Artist;
        album.Text = audioProperties?.Album;

        mediaElement.Play();
    }
}
