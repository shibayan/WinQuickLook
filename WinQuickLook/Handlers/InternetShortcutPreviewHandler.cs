using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var content = File.ReadAllLines(fileName);

            var urlEntry = content.FirstOrDefault(x => x.StartsWith("URL"));

            if (urlEntry == null)
            {
                return (null, default, null);
            }

            var url = urlEntry.Substring(4);

            var requestSize = new Size
            {
                Width = 1200,
                Height = 900
            };

            var webView = new WebBrowser();

            webView.Loaded += (sender, e) => ((WebBrowser)sender).Navigate(url);

            return (webView, requestSize, url);
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".url"
        };
    }
}
