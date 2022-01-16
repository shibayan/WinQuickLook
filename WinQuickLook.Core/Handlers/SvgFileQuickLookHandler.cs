using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public class SvgFileQuickLookHandler : FileQuickLookHandler
{
    protected override IReadOnlyList<string> SupportedExtensions => new[] { ".svg", ".svgz" };

    protected override (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
