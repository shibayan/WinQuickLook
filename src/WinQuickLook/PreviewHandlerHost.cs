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
        
        public PreviewHandlerHost()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        
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

            var ext = Registry.ClassesRoot.OpenSubKey(extension);

            if (ext != null)
            {
                var test = ext.OpenSubKey(PreviewHandlerSubKey);

                if (test != null)
                {
                    return new Guid(Convert.ToString(test.GetValue(null)));
                }

                var className = Convert.ToString(ext.GetValue(null));

                test = Registry.ClassesRoot.OpenSubKey(className + PreviewHandlerSubKey);

                if (test != null)
                {
                    return new Guid(Convert.ToString(test.GetValue(null)));
                }
            }

            return Guid.Empty;
        }

        public bool Open(string fileName)
        {
            UnloadPreviewHandler();
            
            try
            {
                _previewHandler = CreatePreviewHandler(fileName);

                if (_previewHandler != null)
                {
                    _previewHandler.SetWindow(Handle, ClientRectangle);
                    _previewHandler.DoPreview();

                    return true;
                }
            }
            catch
            {
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
                IShellItem shellItem;
                NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, typeof(IShellItem).GUID, out shellItem);

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
