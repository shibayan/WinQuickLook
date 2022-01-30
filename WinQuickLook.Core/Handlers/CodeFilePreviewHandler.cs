using System.IO;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class CodeFilePreviewHandler : FilePreviewHandler
{
    static CodeFilePreviewHandler()
    {
        var highlightingManager = HighlightingManager.Instance;

        highlightingManager.RegisterHighlighting("TypeScript", new[] { ".ts" }, highlightingManager.GetDefinitionByExtension(".js"));
        highlightingManager.RegisterHighlighting("Vue", new[] { ".vue" }, highlightingManager.GetDefinitionByExtension(".html"));
    }

    protected override bool TryCreateViewer(FileInfo fileInfo, out HandlerResult? handlerResult)
    {
        var highlighting = HighlightingManager.Instance.GetDefinitionByExtension(fileInfo.Extension);

        if (highlighting is null)
        {
            handlerResult = default;

            return true;
        }

        var textEditor = new TextEditor();

        using (textEditor.Initialize())
        {
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.FontSize = 14;
            textEditor.IsReadOnly = true;
            textEditor.ShowLineNumbers = true;
            textEditor.SyntaxHighlighting = highlighting;

            textEditor.Load(fileInfo.OpenReadNoLock());
        }

        handlerResult = new HandlerResult { Viewer = textEditor };

        return true;
    }
}
