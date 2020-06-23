﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WinQuickLook.Interop;

namespace WinQuickLook.Handlers
{
    public class TextPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size) GetViewer(string fileName, Size monitorSize)
        {
            var maxWidth = monitorSize.Width - 100;
            var maxHeight = monitorSize.Height - 100;

            var contents = File.ReadAllBytes(fileName);
            var encoding = DetectEncoding(contents);

            var requestSize = new Size
            {
                Width = maxWidth / 2,
                Height = maxHeight / 2
            };

            var textBox = new TextBox();

            textBox.BeginInit();
            textBox.Text = encoding.GetString(contents);
            textBox.IsReadOnly = true;
            textBox.IsReadOnlyCaretVisible = false;
            textBox.FontFamily = new FontFamily("Consolas");
            textBox.FontSize = 16;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.BorderThickness = new Thickness(0);
            textBox.EndInit();

            return (textBox, requestSize);
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".txt", ".log", ".md", ".markdown", ".xml", ".yml", ".yaml", ".config", ".gitignore", ".gitattributes", ".sh", ".bat", ".cmd"
        };

        private static Encoding DetectEncoding(byte[] contents)
        {
            if (contents.Length == 0)
            {
                return Encoding.ASCII;
            }

            var multiLanguage2 = (IMultiLanguage2)Activator.CreateInstance(CLSID.MultiLanguageType);

            int scores = 1;
            int length = contents.Length;

            DetectEncodingInfo encodingInfo;

            var handle = GCHandle.Alloc(contents, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(contents, 0);

            try
            {
                multiLanguage2.DetectInputCodepage(0, 0, ptr, length, out encodingInfo, ref scores);
            }
            finally
            {
                handle.Free();
            }

            Marshal.FinalReleaseComObject(multiLanguage2);

            return Encoding.GetEncoding(encodingInfo.nCodePage);
        }
    }
}
