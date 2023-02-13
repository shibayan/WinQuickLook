using System.IO;

using ICSharpCode.AvalonEdit.Highlighting;

using Microsoft.Web.WebView2.Core;

using WinQuickLook.Extensions;

namespace WinQuickLook.Controls;

public partial class MarkdownFileControl
{
    public MarkdownFileControl() => InitializeComponent();

    public async void Open(FileInfo fileInfo)
    {
        textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("MarkDown");
        textEditor.Load(fileInfo.OpenReadNoLock());

        await webView2.EnsureCoreWebView2Async();
    }

    private void WebView2_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        webView2.NavigateToString(Markdig.Markdown.ToHtml(textEditor.Text));
    }
}
