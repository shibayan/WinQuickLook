using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Web.WebView2.Wpf;

namespace WinQuickLook.Handlers
{
    public class InternetShortcutQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var content = await File.ReadAllLinesAsync(fileName);

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

            var webView2 = new WebView2();

            webView2.Loaded += (sender, e) => ((WebView2)sender).CoreWebView2.Navigate(url);

            return (webView2, requestSize, url);
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".url"
        };
    }
}
