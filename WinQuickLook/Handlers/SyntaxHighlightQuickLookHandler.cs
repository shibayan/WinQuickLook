using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class SyntaxHighlightQuickLookHandler : IQuickLookHandler
    {
        public SyntaxHighlightQuickLookHandler()
        {
            HighlightingManager.Instance.RegisterHighlighting("Vue", new[] { ".ts" }, HighlightingManager.Instance.GetDefinitionByExtension(".js"));
            HighlightingManager.Instance.RegisterHighlighting("Vue", new[] { ".vue" }, HighlightingManager.Instance.GetDefinitionByExtension(".html"));
        }

        public bool CanOpen(string fileName)
        {
            return HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName)) != null;
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var requestSize = new Size
            {
                Width = 1200,
                Height = 900
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

            return (avalonEdit, requestSize, WinExplorerHelper.GetFileSize(fileName));
        }
    }
}
