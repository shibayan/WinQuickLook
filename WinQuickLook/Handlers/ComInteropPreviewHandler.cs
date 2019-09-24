﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;

namespace WinQuickLook.Handlers
{
    public class ComInteropPreviewHandler : IQuickLookPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            return PreviewHandlerHost.GetPreviewHandlerCLSID(fileName) != Guid.Empty;
        }

        public FrameworkElement GetElement(string fileName)
        {
            var maxWidth = SystemParameters.WorkArea.Width - 100;
            var maxHeight = SystemParameters.WorkArea.Height - 100;
            
            var previewHandlerHost = new PreviewHandlerHost();

            var windowsFormsHost = new WindowsFormsHost();

            windowsFormsHost.BeginInit();
            windowsFormsHost.Child = previewHandlerHost;
            windowsFormsHost.Width = maxWidth / 1.5;
            windowsFormsHost.Height = maxHeight / 1.5;
            windowsFormsHost.EndInit();

            previewHandlerHost.Open(fileName);

            return windowsFormsHost;
        }
    }
}
