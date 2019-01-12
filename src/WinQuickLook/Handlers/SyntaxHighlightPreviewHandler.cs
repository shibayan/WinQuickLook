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
            if (!File.Exists(fileName))
            {
                return false;
            }

            return HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName)) != null;
        }

        public override FrameworkElement GetElement(string fileName)
        {
            var maxWidth = SystemParameters.WorkArea.Width - 100;
            var maxHeight = SystemParameters.WorkArea.Height - 100;

            var avalonEdit = new ICSharpCode.AvalonEdit.TextEditor();

            avalonEdit.BeginInit();

            avalonEdit.Width = maxWidth / 2;
            avalonEdit.Height = maxHeight / 2;
            avalonEdit.FontFamily = new FontFamily("Consolas");
            avalonEdit.FontSize = 14;
            avalonEdit.IsReadOnly = true;
            avalonEdit.ShowLineNumbers = true;
            avalonEdit.Load(fileName);
            avalonEdit.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));

            avalonEdit.EndInit();

            return avalonEdit;
        }
    }
}
