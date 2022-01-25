using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;

using Windows.Win32;
using Windows.Win32.Globalization;

using ICSharpCode.AvalonEdit;

using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class TextFilePreviewHandler : FilePreviewHandler
{
    protected override bool CanOpen(FileInfo fileInfo)
    {
        var pnScores = 1;

        using var fileStream = fileInfo.OpenReadNoLock();

        var multiLanguage = (IMultiLanguage2)Activator.CreateInstance(CLSID.CMultiLanguageType)!;

        try
        {
            return multiLanguage.DetectCodepageInIStream(0, 0, new StreamWrapper(fileStream), out _, ref pnScores).Value >= 0;
        }
        finally
        {
            Marshal.ReleaseComObject(multiLanguage);
        }
    }

    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var textEditor = new TextEditor();

        using (textEditor.Init())
        {
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.FontSize = 14;
            textEditor.IsReadOnly = true;
            textEditor.ShowLineNumbers = true;

            textEditor.Load(fileInfo.OpenReadNoLock());
        }

        return new HandlerResult { Viewer = textEditor };
    }
}
