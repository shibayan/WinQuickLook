using System.IO;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class AudioFilePreviewHandler : FilePreviewHandler
{
    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var audioFileControl = new AudioFileControl();

        using (audioFileControl.Init())
        {
            audioFileControl.Open(fileInfo);
        }

        return new HandlerResult { Viewer = audioFileControl };
    }
}
