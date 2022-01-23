using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinQuickLook.Handlers;

public abstract class FilePreviewHandler : IFileSystemPreviewHandler
{
    public bool CanOpen(FileSystemInfo fileSystemInfo) => fileSystemInfo is FileInfo fileInfo && CanOpen(fileInfo);

    public HandlerResult CreateViewer(FileSystemInfo fileSystemInfo) => CreateViewer((FileInfo)fileSystemInfo);

    protected virtual bool CanOpen(FileInfo fileInfo) => SupportedExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);

    protected abstract HandlerResult CreateViewer(FileInfo fileInfo);

    protected virtual IReadOnlyList<string> SupportedExtensions => Array.Empty<string>();
}
