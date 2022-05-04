using System.IO;
using System.Windows.Controls;

using WinQuickLook.Extensions;

namespace WinQuickLook.Controls;

public partial class HtmlFileControl : UserControl
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
