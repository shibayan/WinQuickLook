using System.IO;
using System.Runtime.InteropServices;

using Windows.Win32;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class MediaFilePreviewHandler : FilePreviewHandler
{
    protected override bool CanOpen(FileInfo fileInfo)
    {
        if (PInvoke.MFCreateSourceReaderFromURL(fileInfo.FullName, null, out var sourceReader).Value < 0)
        {
            return false;
        }

        Marshal.ReleaseComObject(sourceReader);

        return true;
    }

    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        if (PInvoke.MFCreateSourceReaderFromURL(fileInfo.FullName, null, out var sourceReader).Value < 0)
        {
            throw new System.Exception();
        }

        try
        {
            if (sourceReader.GetCurrentMediaType(PInvoke.MF_SOURCE_READER_FIRST_VIDEO_STREAM, out var videoMediaType).Value >= 0)
            {
                Marshal.ReleaseComObject(videoMediaType);

                return CreateVideoViewer(fileInfo);
            }

            if (sourceReader.GetCurrentMediaType(PInvoke.MF_SOURCE_READER_FIRST_AUDIO_STREAM, out var audioMediaType).Value >= 0)
            {
                Marshal.ReleaseComObject(audioMediaType);

                return CreateAudioViewer(fileInfo);
            }
        }
        finally
        {
            Marshal.ReleaseComObject(sourceReader);
        }

        throw new System.Exception();
    }

    private HandlerResult CreateAudioViewer(FileInfo fileInfo)
    {
        var audioFileControl = new AudioFileControl();

        using (audioFileControl.Init())
        {
            audioFileControl.Open(fileInfo);
        }

        return new HandlerResult { Viewer = audioFileControl };
    }

    private HandlerResult CreateVideoViewer(FileInfo fileInfo)
    {
        var videoFileControl = new VideoFileControl();

        using (videoFileControl.Init())
        {
            videoFileControl.Open(fileInfo);
        }

        return new HandlerResult { Viewer = videoFileControl };
    }
}
