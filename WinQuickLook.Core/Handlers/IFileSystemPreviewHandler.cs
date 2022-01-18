using System.IO;

namespace WinQuickLook.Handlers;

public interface IFileSystemPreviewHandler
{
    bool CanOpen(FileSystemInfo fileSystemInfo);

    HandlerResult CreateViewer(FileSystemInfo fileSystemInfo);
}
