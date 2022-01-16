using System.IO;

namespace WinQuickLook.Handlers;

public abstract class DirectoryQuickLookHandler : IQuickLookHandler
{
    public bool CanOpen(FileSystemInfo fileSystemInfo) => CanOpen((DirectoryInfo)fileSystemInfo);

    public HandlerResult CreateViewer(FileSystemInfo fileSystemInfo) => CreateViewer((DirectoryInfo)fileSystemInfo);

    protected abstract bool CanOpen(DirectoryInfo directoryInfo);

    protected abstract HandlerResult CreateViewer(DirectoryInfo directoryInfo);
}
