using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public class HtmlFileQuickLookHandler : FileQuickLookHandler
{
    protected override IReadOnlyList<string> SupportedExtensions => new[] { ".htm", ".html", ".xhtml" };

    protected override (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
