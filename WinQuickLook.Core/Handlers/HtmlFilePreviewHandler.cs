﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class HtmlFilePreviewHandler : FilePreviewHandler
{
    protected override bool TryCreateViewer(FileInfo fileInfo, out HandlerResult? handlerResult)
    {
        if (!s_supportedExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
        {
            handlerResult = default;

            return false;
        }

        var htmlFileControl = new HtmlFileControl();

        using (htmlFileControl.Initialize())
        {
            htmlFileControl.Open(fileInfo);
        }

        handlerResult = new HandlerResult { Viewer = htmlFileControl };

        return true;
    }

    private static readonly IReadOnlyList<string> s_supportedExtensions = new[] { ".htm", ".html", ".xhtml", ".md", ".markdown" };
}
