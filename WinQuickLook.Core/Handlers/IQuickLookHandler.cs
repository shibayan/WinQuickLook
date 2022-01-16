using System.IO;

namespace WinQuickLook.Handlers;

public interface IQuickLookHandler
{
    bool CanOpen(FileSystemInfo fileSystemInfo);

    HandlerResult CreateViewer(FileSystemInfo fileSystemInfo);
}
