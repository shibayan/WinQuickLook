using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class TextQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            var extension = fileInfo.Extension.ToLower();

            return _supportFormats.Contains(extension);
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
            avalonEdit.Load(fileInfo.OpenRead());
            avalonEdit.EndInit();

            return (avalonEdit, requestSize, WinExplorerHelper.GetSizeFormat(fileInfo.Length));
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".txt", ".log", ".md", ".markdown", ".xml", ".yml", ".yaml", ".config", ".gitignore", ".gitattributes", ".sh", ".bat", ".cmd"
        };
    }
}
