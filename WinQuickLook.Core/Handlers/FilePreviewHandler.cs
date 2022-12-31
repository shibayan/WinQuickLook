using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace WinQuickLook.Handlers;

public abstract class FilePreviewHandler : IFileSystemPreviewHandler
{
    public abstract HandlerPriorityClass PriorityClass { get; }

    public bool TryCreateViewer(FileSystemInfo fileSystemInfo, [NotNullWhen(true)] out HandlerResult? handlerResult)
    {
        if (fileSystemInfo is FileInfo fileInfo)
        {
            if (PriorityClass == HandlerPriorityClass.Lowest)
            {
                return TryCreateViewer(fileInfo, out handlerResult);
            }

            if (!s_genericFileExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
            {
                return TryCreateViewer(fileInfo, out handlerResult);
            }
        }

        handlerResult = default;

        return false;
    }

    protected abstract bool TryCreateViewer(FileInfo fileInfo, out HandlerResult? handlerResult);

    private static readonly IReadOnlyList<string> s_genericFileExtensions = new[] { ".dll", ".exe" };
}
