using System.IO;

using Cylinder;

namespace WinQuickLook.Controls;

public partial class GenericDirectoryControl
{
    public GenericDirectoryControl() => InitializeComponent();

    public Ref<DirectoryInfo> DirectoryInfo { get; } = new(null);

    public void Open(DirectoryInfo directoryInfo)
    {
        DirectoryInfo.Value = directoryInfo;
    }
}
