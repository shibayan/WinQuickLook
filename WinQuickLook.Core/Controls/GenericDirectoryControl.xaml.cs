using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WinQuickLook.Controls;

public partial class GenericDirectoryControl : UserControl
{
    public GenericDirectoryControl()
    {
        InitializeComponent();
    }

    public DirectoryInfo DirectoryInfo
    {
        get => (DirectoryInfo)GetValue(DirectoryInfoProperty);
        set => SetValue(DirectoryInfoProperty, value);
    }

    public static readonly DependencyProperty DirectoryInfoProperty =
        DependencyProperty.Register(nameof(DirectoryInfo), typeof(DirectoryInfo), typeof(GenericDirectoryControl), new PropertyMetadata(null));

    public void Open(DirectoryInfo directoryInfo)
    {
        throw new NotImplementedException();
    }
}
