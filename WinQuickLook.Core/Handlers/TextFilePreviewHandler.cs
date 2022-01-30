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
    protected override bool TryCreateViewer(FileInfo fileInfo, out HandlerResult? handlerResult)
    {
        var pnScores = 1;

        using var fileStream = fileInfo.OpenReadNoLock();

        var multiLanguage = (IMultiLanguage2)Activator.CreateInstance(CLSID.CMultiLanguageType)!;

        try
        {
            if (multiLanguage.DetectCodepageInIStream(0, 0, new StreamWrapper(fileStream), out _, ref pnScores).Value < 0)
            {
                handlerResult = default;

                return false;
            }
        }
        finally
        {
            Marshal.ReleaseComObject(multiLanguage);
        }

        var textEditor = new TextEditor();

        using (textEditor.Initialize())
        {
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.FontSize = 14;
            textEditor.IsReadOnly = true;
            textEditor.ShowLineNumbers = true;

            textEditor.Load(fileInfo.OpenReadNoLock());
        }

        handlerResult = new HandlerResult { Viewer = textEditor };

        return true;
    }
}
