using System.IO;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class VideoFilePreviewHandler : FilePreviewHandler
{
    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var videoFileControl = new VideoFileControl();

        using (videoFileControl.Init())
        {
            videoFileControl.Open(fileInfo);
        }

        return new HandlerResult { Viewer = videoFileControl };
    }
}
