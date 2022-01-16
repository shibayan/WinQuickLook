using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public class PdfFileQuickLookHandler : FileQuickLookHandler
{
    protected override IReadOnlyList<string> SupportedExtensions => new[] { ".pdf" };

    protected override (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
