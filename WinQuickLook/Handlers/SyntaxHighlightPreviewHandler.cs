using System.IO;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;

namespace WinQuickLook.Handlers
{
    public class SyntaxHighlightPreviewHandler : IPreviewHandler
    {
        public SyntaxHighlightPreviewHandler()
        {
            HighlightingManager.Instance.RegisterHighlighting("Vue", new[] { ".ts" }, HighlightingManager.Instance.GetDefinitionByExtension(".js"));
            HighlightingManager.Instance.RegisterHighlighting("Vue", new[] { ".vue" }, HighlightingManager.Instance.GetDefinitionByExtension(".html"));
        }

        public bool CanOpen(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            return HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName)) != null;
        }

        public (FrameworkElement, Size) GetViewer(string fileName, Size monitorSize)
        {
            var maxWidth = monitorSize.Width - 100;
            var maxHeight = monitorSize.Height - 100;

            var requestSize = new Size
            {
                Width = maxWidth / 2,
                Height = maxHeight / 2
            };

            var avalonEdit = new ICSharpCode.AvalonEdit.TextEditor();

            avalonEdit.BeginInit();
            avalonEdit.FontFamily = new FontFamily("Consolas");
            avalonEdit.FontSize = 14;
            avalonEdit.IsReadOnly = true;
            avalonEdit.ShowLineNumbers = true;
            avalonEdit.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));
            avalonEdit.Load(fileName);
            avalonEdit.EndInit();

            return (avalonEdit, requestSize);
        }
    }
}
