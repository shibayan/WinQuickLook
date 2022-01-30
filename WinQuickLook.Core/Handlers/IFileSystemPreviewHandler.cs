using System.IO;

namespace WinQuickLook.Handlers;

public interface IFileSystemPreviewHandler
{
    bool TryCreateViewer(FileSystemInfo fileSystemInfo, out HandlerResult? handlerResult);
}
