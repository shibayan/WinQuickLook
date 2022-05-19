using System.IO;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class GenericFilePreviewHandler : FilePreviewHandler
{
    public override HandlerPriorityClass PriorityClass => HandlerPriorityClass.Lowest;

    protected override bool TryCreateViewer(FileInfo fileInfo, out HandlerResult? handlerResult)
    {
        var fileControl = new GenericFileControl();

        using (fileControl.Initialize())
        {
            fileControl.Open(fileInfo);
        }

        handlerResult = new HandlerResult { Viewer = fileControl };

        return true;
    }
}
