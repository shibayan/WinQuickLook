﻿using System;
using System.Collections.Generic;
using System.IO;

namespace WinQuickLook.Handlers;

public class SvgFilePreviewHandler : FilePreviewHandler
{
    protected override IReadOnlyList<string> SupportedExtensions => new[] { ".svg", ".svgz" };

    protected override HandlerResult CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}