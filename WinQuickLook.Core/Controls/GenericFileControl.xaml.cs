using System;
using System.IO;
using System.Windows;

namespace WinQuickLook.Controls;

public partial class GenericFileControl
{
    public GenericFileControl()
    {
        InitializeComponent();
    }

    public FileInfo FileInfo
    {
        get => (FileInfo)GetValue(FileInfoProperty);
        set => SetValue(FileInfoProperty, value);
    }

    public static readonly DependencyProperty FileInfoProperty =
        DependencyProperty.Register(nameof(FileInfo), typeof(FileInfo), typeof(GenericFileControl), new PropertyMetadata(null));

    public void Open(FileInfo fileInfo)
    {
        throw new NotImplementedException();
    }
}
