using System.IO;
using System.Windows;

namespace WinQuickLook.Controls;

public partial class GenericDirectoryControl
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
        DirectoryInfo = directoryInfo;
    }
}
