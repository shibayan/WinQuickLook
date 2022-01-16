using System.IO;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

using WinQuickLook.Extensions;

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

    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var textEditor = new TextEditor();

        using (textEditor.Init())
        {
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.FontSize = 14;
            textEditor.IsReadOnly = true;
            textEditor.ShowLineNumbers = true;
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(fileInfo.Extension);

            textEditor.Load(fileInfo.OpenReadNoLock());
        }

        return new HandlerResult { Viewer = textEditor };
    }
}
