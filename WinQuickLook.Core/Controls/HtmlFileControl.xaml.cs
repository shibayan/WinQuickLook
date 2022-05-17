using System.IO;

using WinQuickLook.Extensions;

namespace WinQuickLook.Controls;

public partial class HtmlFileControl
{
    public HtmlFileControl()
    {
        InitializeComponent();
    }

    public void Open(FileInfo fileInfo)
    {
        textEditor.Load(fileInfo.OpenReadNoLock());
    }
}
