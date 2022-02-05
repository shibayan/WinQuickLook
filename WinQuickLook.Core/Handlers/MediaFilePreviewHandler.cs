using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

using Windows.Win32;
using Windows.Win32.Media.MediaFoundation;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class MediaFilePreviewHandler : FilePreviewHandler
{
    protected override bool TryCreateViewer(FileInfo fileInfo, out HandlerResult? handlerResult)
    {
        if (PInvoke.MFCreateSourceReaderFromURL(fileInfo.FullName, null, out var sourceReader).Failed)
        {
            handlerResult = default;

            return false;
        }

        try
        {
            if (sourceReader.GetCurrentMediaType(PInvoke.MF_SOURCE_READER_FIRST_VIDEO_STREAM, out var videoMediaType).Succeeded)
            {
                try
                {
                    return TryCreateVideoViewer(fileInfo, videoMediaType, out handlerResult);
                }
                finally
                {
                    Marshal.ReleaseComObject(videoMediaType);
                }
            }

            if (sourceReader.GetCurrentMediaType(PInvoke.MF_SOURCE_READER_FIRST_AUDIO_STREAM, out var audioMediaType).Succeeded)
            {
                try
                {
                    return TryCreateAudioViewer(fileInfo, out handlerResult);
                }
                finally
                {
                    Marshal.ReleaseComObject(audioMediaType);
                }
            }
        }
        finally
        {
            Marshal.ReleaseComObject(sourceReader);
        }

        handlerResult = default;

        return false;
    }

    private static bool TryCreateVideoViewer(FileInfo fileInfo, IMFAttributes videoMediaType, [NotNullWhen(true)] out HandlerResult? handlerResult)
    {
        if (!TryGetVideoSize(videoMediaType, out var videoSize))
        {
            handlerResult = null;

            return false;
        }

        var size = videoSize.FitTo(1200);

        var videoFileControl = new VideoFileControl();

        using (videoFileControl.Initialize())
        {
            videoFileControl.Open(fileInfo);
        }

        handlerResult = new HandlerResult { Viewer = videoFileControl };

        return true;
    }

    private static bool TryCreateAudioViewer(FileInfo fileInfo, [NotNullWhen(true)] out HandlerResult? handlerResult)
    {
        var audioFileControl = new AudioFileControl();

        using (audioFileControl.Initialize())
        {
            audioFileControl.Open(fileInfo);
        }

        handlerResult = new HandlerResult { Viewer = audioFileControl };

        return true;
    }

    private static bool TryGetVideoSize(IMFAttributes videoMediaType, out Size size)
    {
        if (PInvoke.MFGetAttributeSize(videoMediaType, out var width, out var height).Failed)
        {
            size = default;

            return false;
        }

        size = new Size(width, height);

        return true;
    }
}
