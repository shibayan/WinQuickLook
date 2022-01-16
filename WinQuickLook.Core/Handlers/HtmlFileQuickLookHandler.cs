﻿using System;
using System.Collections.Generic;
using System.IO;

namespace WinQuickLook.Handlers;

public class HtmlFileQuickLookHandler : FileQuickLookHandler
{
    protected override IReadOnlyList<string> SupportedExtensions => new[] { ".htm", ".html", ".xhtml" };

    protected override HandlerResult CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
