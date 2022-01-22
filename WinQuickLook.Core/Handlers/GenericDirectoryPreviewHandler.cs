using System.IO;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class GenericDirectoryPreviewHandler : DirectoryPreviewHandler
{
    protected override bool CanOpen(DirectoryInfo directoryInfo) => true;

    protected override HandlerResult CreateViewer(DirectoryInfo directoryInfo)
    {
        var directoryControl = new GenericDirectoryControl();

        using (directoryControl.Init())
        {
            directoryControl.Open(directoryInfo);
        }

        return new HandlerResult { Viewer = directoryControl };
    }
}
