using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WinQuickLook.Handlers
{
    public class InternetShortcutPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size) GetViewer(string fileName)
        {
            var content = File.ReadAllLines(fileName);

            var url = content.FirstOrDefault(x => x.StartsWith("URL"));

            if (url == null)
            {
                return (null, default);
            }

            var requestSize = new Size
            {
                Width = 1200,
                Height = 900
            };

            var webView = new WebBrowser();

            webView.Loaded += (sender, e) => ((WebBrowser)sender).Navigate(url.Substring(4));

            return (webView, requestSize);
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".url"
        };
    }
}
