using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using WinQuickLook.Interop;

namespace WinQuickLook.Handlers
{
    public class SyntaxHighlightPreviewHandler : PreviewHandlerBase
    {
        public SyntaxHighlightPreviewHandler()
        {
            var styleStream = Application.GetResourceStream(new Uri("Resources/prism.css", UriKind.Relative));

            using (var reader = new StreamReader(styleStream.Stream))
            {
                _style = reader.ReadToEnd();
            }

            var scriptStream = Application.GetResourceStream(new Uri("Resources/prism.js", UriKind.Relative));

            using (var reader = new StreamReader(scriptStream.Stream))
            {
                _script = reader.ReadToEnd();
            }
        }

        private readonly string _style;
        private readonly string _script;

        public override bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.ContainsKey(extension);
        }

        public override FrameworkElement GetElement(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            var maxWidth = SystemParameters.WorkArea.Width - 100;
            var maxHeight = SystemParameters.WorkArea.Height - 100;

            var contents = File.ReadAllBytes(fileName);
            var encoding = DetectEncoding(contents);

            var webBrowser = new WebBrowser();

            webBrowser.BeginInit();
            webBrowser.Width = maxWidth / 2;
            webBrowser.Height = maxHeight / 2;
            webBrowser.EndInit();

            var content = new StringBuilder();

            content.Append("<!DOCTYPE html><html><head><style>");
            content.Append(_style);
            content.Append($"</style></head><body><pre><code class=\"language-{_supportFormats[extension]}\">");
            content.Append(encoding.GetString(contents));
            content.Append("</code></pre><script>");
            content.Append(_script);
            content.Append("</script></body></html>");

            webBrowser.NavigateToString(content.ToString());

            return webBrowser;
        }

        private static readonly Dictionary<string, string> _supportFormats = new Dictionary<string, string>
        {
            { ".cs", "csharp" },
            { ".cpp", "cpp" },
            { ".cxx", "cpp" },
            { ".js", "javascript" },
            { ".json", "json" },
            { ".php", "php" },
            { ".sql", "sql" },
            { ".yml", "yaml" },
            { ".yaml", "yaml" }
        };

        private static Encoding DetectEncoding(byte[] contents)
        {
            if (contents.Length == 0)
            {
                return Encoding.ASCII;
            }

            var multiLanguage2 = (IMultiLanguage2)Activator.CreateInstance(CLSID.MultiLanguageType);

            int scores = 1;
            int length = contents.Length;

            DetectEncodingInfo encodingInfo;

            var handle = GCHandle.Alloc(contents, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(contents, 0);

            try
            {
                multiLanguage2.DetectInputCodepage(0, 0, ptr, length, out encodingInfo, ref scores);
            }
            finally
            {
                handle.Free();
            }

            Marshal.FinalReleaseComObject(multiLanguage2);

            return Encoding.GetEncoding(encodingInfo.nCodePage);
        }
    }
}
