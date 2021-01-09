using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class TextQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size, string) GetViewer(string fileName)
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
            avalonEdit.Load(fileName);
            avalonEdit.EndInit();

            return (avalonEdit, requestSize, WinExplorerHelper.GetFileSize(fileName));
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".txt", ".log", ".md", ".markdown", ".xml", ".yml", ".yaml", ".config", ".gitignore", ".gitattributes", ".sh", ".bat", ".cmd"
        };
    }
}
