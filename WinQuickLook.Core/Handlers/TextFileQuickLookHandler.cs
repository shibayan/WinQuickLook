using System.IO;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;

using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class TextFileQuickLookHandler : FileQuickLookHandler
{
    protected override bool CanOpen(FileInfo fileInfo)
    {
        return false;
    }

    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var textEditor = new TextEditor();

        using (textEditor.Init())
        {
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.FontSize = 14;
            textEditor.IsReadOnly = true;
            textEditor.ShowLineNumbers = true;

            textEditor.Load(fileInfo.OpenReadNoLock());
        }

        return new HandlerResult { Viewer = textEditor };
    }
}
