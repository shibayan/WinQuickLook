using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace WinQuickLook.Handlers;

public abstract class FilePreviewHandler : IFileSystemPreviewHandler
{
    public abstract HandlerPriorityClass PriorityClass { get; }

    public bool TryCreateViewer(FileSystemInfo fileSystemInfo, [NotNullWhen(true)] out HandlerResult? handlerResult)
    {
        if (fileSystemInfo is FileInfo fileInfo)
        {
            return TryCreateViewer(fileInfo, out handlerResult);
        }

        handlerResult = default;

        return false;
    }

    protected abstract bool TryCreateViewer(FileInfo fileInfo, out HandlerResult? handlerResult);
}
