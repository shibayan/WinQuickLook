using System.IO;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;

namespace WinQuickLook.Handlers
{
    public class SyntaxHighlightPreviewHandler : PreviewHandlerBase
    {
        public override bool CanOpen(string fileName)
        {
            return HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName)) != null;
        }

        public override FrameworkElement GetElement(string fileName)
        {
            var avalonEdit = new ICSharpCode.AvalonEdit.TextEditor();

            avalonEdit.BeginInit();

            avalonEdit.FontFamily = new FontFamily("Consolas");
            avalonEdit.FontSize = 14;
            avalonEdit.IsReadOnly = true;
            avalonEdit.Load(fileName);
            avalonEdit.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));

            avalonEdit.EndInit();

            return avalonEdit;
        }
    }
}
