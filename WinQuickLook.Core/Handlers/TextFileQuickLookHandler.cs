﻿using System;
using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public class TextFileQuickLookHandler : FileQuickLookHandler
{
    protected override bool CanOpen(FileInfo fileInfo)
    {
        return false;
    }

    protected override (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
