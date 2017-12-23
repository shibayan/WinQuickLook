using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Win32;

using WinQuickLook.Interop;

namespace WinQuickLook
{
    public class PreviewHandlerHost : Control
    {
        private IPreviewHandler _previewHandler;

        private const string PreviewHandlerSubKey = "shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}";
        
        protected override void Dispose(bool disposing)
        {
            UnloadPreviewHandler();

            base.Dispose(disposing);
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _previewHandler?.SetRect(ClientRectangle);
        }

        public static Guid GetPreviewHandlerCLSID(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
            {
                return Guid.Empty;
            }

            var extensionKey = Registry.ClassesRoot.OpenSubKey(extension);

            if (extensionKey != null)
            {
                var subKey = extensionKey.OpenSubKey(PreviewHandlerSubKey);

                if (subKey != null)
                {
                    return new Guid(Convert.ToString(subKey.GetValue(null)));
                }

                var className = Convert.ToString(extensionKey.GetValue(null));

                subKey = Registry.ClassesRoot.OpenSubKey(className + PreviewHandlerSubKey);

                if (subKey != null)
                {
                    return new Guid(Convert.ToString(subKey.GetValue(null)));
                }
            }

            return Guid.Empty;
        }

        public bool Open(string fileName)
        {
            UnloadPreviewHandler();

            _previewHandler = CreatePreviewHandler(fileName);

            if (_previewHandler != null)
            {
                _previewHandler.SetWindow(Handle, ClientRectangle);
                _previewHandler.DoPreview();

                return true;
            }

            return false;
        }

        public IPreviewHandler CreatePreviewHandler(string fileName)
        {
            var clsid = GetPreviewHandlerCLSID(fileName);

            if (clsid == Guid.Empty)
            {
                return null;
            }

            var type = Type.GetTypeFromCLSID(clsid);

            var previewHandler = (IPreviewHandler)Activator.CreateInstance(type);

            var initializeWithFile = previewHandler.QueryInterface<IInitializeWithFile>();

            if (initializeWithFile != null)
            {
                initializeWithFile.Initialize(fileName, 0);

                return previewHandler;
            }

            var initializeWithItem = previewHandler.QueryInterface<IInitializeWithItem>();

            if (initializeWithItem != null)
            {
                NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, typeof(IShellItem).GUID, out var shellItem);

                initializeWithItem.Initialize(shellItem, 0);

                return previewHandler;
            }

            Marshal.FinalReleaseComObject(previewHandler);

            return null;
        }
        
        private void UnloadPreviewHandler()
        {
            if (_previewHandler != null)
            {
                _previewHandler.Unload();

                Marshal.FinalReleaseComObject(_previewHandler);

                _previewHandler = null;
            }
        }
    }
}
