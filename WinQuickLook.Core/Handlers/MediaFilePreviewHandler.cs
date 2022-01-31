using System.IO;
using System.Runtime.InteropServices;

using Windows.Win32;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class MediaFilePreviewHandler : FilePreviewHandler
{
    protected override bool TryCreateViewer(FileInfo fileInfo, out HandlerResult? handlerResult)
    {
        if (PInvoke.MFCreateSourceReaderFromURL(fileInfo.FullName, null, out var sourceReader).Value < 0)
        {
            handlerResult = default;

            return false;
        }

        try
        {
            if (sourceReader.GetCurrentMediaType(PInvoke.MF_SOURCE_READER_FIRST_VIDEO_STREAM, out var videoMediaType).Value >= 0)
            {
                Marshal.ReleaseComObject(videoMediaType);

                handlerResult = CreateVideoViewer(fileInfo);

                return true;
            }

            if (sourceReader.GetCurrentMediaType(PInvoke.MF_SOURCE_READER_FIRST_AUDIO_STREAM, out var audioMediaType).Value >= 0)
            {
                Marshal.ReleaseComObject(audioMediaType);

                handlerResult = CreateAudioViewer(fileInfo);

                return true;
            }
        }
        finally
        {
            Marshal.ReleaseComObject(sourceReader);
        }

        handlerResult = default;

        return false;
    }

    private static HandlerResult CreateAudioViewer(FileInfo fileInfo)
    {
        var audioFileControl = new AudioFileControl();

        using (audioFileControl.Initialize())
        {
            audioFileControl.Open(fileInfo);
        }

        return new HandlerResult { Viewer = audioFileControl };
    }

    private static HandlerResult CreateVideoViewer(FileInfo fileInfo)
    {
        var videoFileControl = new VideoFileControl();

        using (videoFileControl.Initialize())
        {
            videoFileControl.Open(fileInfo);
        }

        return new HandlerResult { Viewer = videoFileControl };
    }
}
