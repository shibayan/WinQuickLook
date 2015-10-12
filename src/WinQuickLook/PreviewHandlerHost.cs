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
            
            var guid = GetPreviewHandlerCLSID(fileName);

            if (guid == Guid.Empty)
            {
                return false;
            }

            try
            {
                var type = Type.GetTypeFromCLSID(guid);

                _previewHandler = (IPreviewHandler)Activator.CreateInstance(type);

                if (_previewHandler is IInitializeWithFile)
                {
                    ((IInitializeWithFile)_previewHandler).Initialize(fileName, 0);
                }
                else if (_previewHandler is IInitializeWithItem)
                {
                    IShellItem shellItem;
                    NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, typeof(IShellItem).GUID, out shellItem);

                    ((IInitializeWithItem)_previewHandler).Initialize(shellItem, 0);
                }

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
        
        public void UnloadPreviewHandler()
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
