using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public class TextFileQuickLookHandler : FileQuickLookHandler
{
    protected override IReadOnlyList<string> SupportedExtensions => new[]
    {
        ".txt", ".log", ".md", ".markdown", ".xml", ".yml", ".yaml", ".config", ".gitignore", ".gitattributes", ".sh", ".bat", ".cmd"
    };

    protected override (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
