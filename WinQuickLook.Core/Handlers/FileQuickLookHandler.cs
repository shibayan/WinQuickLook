using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace WinQuickLook.Handlers;

public abstract class FileQuickLookHandler : IQuickLookHandler
{
    public bool CanOpen(FileSystemInfo fileSystemInfo) => CanOpen((FileInfo)fileSystemInfo);

    public (FrameworkElement, Size, string) CreateViewer(FileSystemInfo fileSystemInfo) => CreateViewer((FileInfo)fileSystemInfo);

    protected virtual bool CanOpen(FileInfo fileInfo) => SupportedExtensions.Contains(fileInfo.Extension.ToLowerInvariant());

    protected abstract (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo);

    protected virtual IReadOnlyList<string> SupportedExtensions { get; } = Array.Empty<string>();
}
