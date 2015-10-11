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
        private IPreviewHandler _currentPreviewHandler;

        private Guid _currentPreviewHandlerGuid;
        
        public PreviewHandlerHost()
        {
            _currentPreviewHandlerGuid = Guid.Empty;

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
        }
        
        protected override void Dispose(bool disposing)
        {
            UnloadPreviewHandler();

            if (_currentPreviewHandler != null)
            {
                Marshal.FinalReleaseComObject(_currentPreviewHandler);

                _currentPreviewHandler = null;
            }

            base.Dispose(disposing);
        }
        
        public static Guid GetPreviewHandlerGUID(string filename)
        {
            var ext = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename));

            if (ext != null)
            {
                var test = ext.OpenSubKey("shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}");

                if (test != null)
                {
                    return new Guid(Convert.ToString(test.GetValue(null)));
                }

                var className = Convert.ToString(ext.GetValue(null));

                test = Registry.ClassesRoot.OpenSubKey(className + "\\shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}");

                if (test != null)
                {
                    return new Guid(Convert.ToString(test.GetValue(null)));
                }
            }

            return Guid.Empty;
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _currentPreviewHandler?.SetRect(ClientRectangle);
        }
        
        public bool Open(string filename)
        {
            UnloadPreviewHandler();
            
            Guid guid = GetPreviewHandlerGUID(filename);

            if (guid != Guid.Empty)
            {
                try
                {
                    if (guid != _currentPreviewHandlerGuid)
                    {
                        _currentPreviewHandlerGuid = guid;

                        if (_currentPreviewHandler != null)
                        {
                            Marshal.FinalReleaseComObject(_currentPreviewHandler);
                        }

                        var comType = Type.GetTypeFromCLSID(_currentPreviewHandlerGuid);

                        _currentPreviewHandler = (IPreviewHandler)Activator.CreateInstance(comType);
                    }

                    if (_currentPreviewHandler is IInitializeWithFile)
                    {
                        ((IInitializeWithFile)_currentPreviewHandler).Initialize(filename, 0);
                    }
                    else if (_currentPreviewHandler is IInitializeWithItem)
                    {
                        IShellItem shellItem;
                        NativeMethods.SHCreateItemFromParsingName(filename, IntPtr.Zero, typeof(IShellItem).GUID, out shellItem);

                        ((IInitializeWithItem)_currentPreviewHandler).Initialize(shellItem, 0);
                    }

                    if (_currentPreviewHandler != null)
                    {
                        _currentPreviewHandler.SetWindow(Handle, ClientRectangle);
                        _currentPreviewHandler.DoPreview();

                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }
        
        public void UnloadPreviewHandler()
        {
            _currentPreviewHandler?.Unload();
        }
    }
}
