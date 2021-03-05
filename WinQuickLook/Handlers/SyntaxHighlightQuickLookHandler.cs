using System.IO;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class SyntaxHighlightQuickLookHandler : IQuickLookHandler
    {
        static SyntaxHighlightQuickLookHandler()
        {
            HighlightingManager.Instance.RegisterHighlighting("Vue", new[] { ".ts" }, HighlightingManager.Instance.GetDefinitionByExtension(".js"));
            HighlightingManager.Instance.RegisterHighlighting("Vue", new[] { ".vue" }, HighlightingManager.Instance.GetDefinitionByExtension(".html"));
        }

        public bool CanOpen(FileInfo fileInfo)
        {
            return HighlightingManager.Instance.GetDefinitionByExtension(fileInfo.Extension) != null;
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
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
            avalonEdit.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(fileInfo.Extension);
            avalonEdit.Load(fileInfo.OpenRead());
            avalonEdit.EndInit();

            return (avalonEdit, requestSize, WinExplorerHelper.GetSizeFormat(fileInfo.Length));
        }
    }
}
