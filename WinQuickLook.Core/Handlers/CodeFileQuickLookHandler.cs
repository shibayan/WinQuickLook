using System;
using System.IO;
using System.Windows;

using ICSharpCode.AvalonEdit.Highlighting;

namespace WinQuickLook.Handlers;

public class CodeFileQuickLookHandler : FileQuickLookHandler
{
    static CodeFileQuickLookHandler()
    {
        var highlightingManager = HighlightingManager.Instance;

        highlightingManager.RegisterHighlighting("TypeScript", new[] { ".ts" }, highlightingManager.GetDefinitionByExtension(".js"));
        highlightingManager.RegisterHighlighting("Vue", new[] { ".vue" }, highlightingManager.GetDefinitionByExtension(".html"));
    }

    protected override bool CanOpen(FileInfo fileInfo) => HighlightingManager.Instance.GetDefinitionByExtension(fileInfo.Extension) is not null;

    protected override (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
