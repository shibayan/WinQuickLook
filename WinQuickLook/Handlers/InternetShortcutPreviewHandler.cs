using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

using Microsoft.Toolkit.Wpf.UI.Controls;

namespace WinQuickLook.Handlers
{
    public class InternetShortcutPreviewHandler : PreviewHandlerBase
    {
        public override bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public override FrameworkElement GetElement(string fileName)
        {
            var content = File.ReadAllLines(fileName);

            var url = content.FirstOrDefault(x => x.StartsWith("URL"));

            if (url == null)
            {
                return null;
            }

            var maxWidth = SystemParameters.WorkArea.Width - 100;
            var maxHeight = SystemParameters.WorkArea.Height - 100;

            var webView = new WebView();

            webView.BeginInit();
            webView.Width = maxWidth / 2;
            webView.Height = maxHeight / 2;
            webView.EndInit();

            webView.Loaded += (sender, e) => ((WebView)sender).Navigate(url.Substring(4));

            return webView;
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".url"
        };
    }
}
