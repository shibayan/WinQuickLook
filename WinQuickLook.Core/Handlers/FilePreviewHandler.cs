using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinQuickLook.Handlers;

public abstract class FilePreviewHandler : IFileSystemPreviewHandler
{
    public bool CanOpen(FileSystemInfo fileSystemInfo) => CanOpen((FileInfo)fileSystemInfo);

    public HandlerResult CreateViewer(FileSystemInfo fileSystemInfo) => CreateViewer((FileInfo)fileSystemInfo);

    protected virtual bool CanOpen(FileInfo fileInfo) => SupportedExtensions.Contains(fileInfo.Extension.ToLowerInvariant());

    protected abstract HandlerResult CreateViewer(FileInfo fileInfo);

    protected virtual IReadOnlyList<string> SupportedExtensions => Array.Empty<string>();
}
