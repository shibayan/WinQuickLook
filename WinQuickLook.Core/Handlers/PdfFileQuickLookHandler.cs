using System;
using System.Collections.Generic;
using System.IO;

namespace WinQuickLook.Handlers;

public class PdfFileQuickLookHandler : FileQuickLookHandler
{
    protected override IReadOnlyList<string> SupportedExtensions => new[] { ".pdf" };

    protected override HandlerResult CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
