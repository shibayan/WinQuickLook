using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public abstract class DirectoryQuickLookHandler : IQuickLookHandler
{
    public bool CanOpen(FileSystemInfo fileSystemInfo) => CanOpen((DirectoryInfo)fileSystemInfo);

    public (FrameworkElement, Size, string) CreateViewer(FileSystemInfo fileSystemInfo) => CreateViewer((DirectoryInfo)fileSystemInfo);

    protected abstract bool CanOpen(DirectoryInfo directoryInfo);

    protected abstract (FrameworkElement, Size, string) CreateViewer(DirectoryInfo directoryInfo);
}
