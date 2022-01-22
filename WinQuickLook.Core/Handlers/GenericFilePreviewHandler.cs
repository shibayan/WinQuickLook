using System.IO;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class GenericFilePreviewHandler : FilePreviewHandler
{
    protected override bool CanOpen(FileInfo fileInfo) => true;

    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var fileControl = new GenericFileControl();

        using (fileControl.Init())
        {
            fileControl.Open(fileInfo);
        }

        return new HandlerResult { Viewer = fileControl };
    }
}
