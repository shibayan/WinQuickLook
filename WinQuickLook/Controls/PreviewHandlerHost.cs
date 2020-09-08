using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Microsoft.AppCenter.Crashes;

using WinQuickLook.Interop;

namespace WinQuickLook.Controls
{
    public class PreviewHandlerHost : Control
    {
        private IPreviewHandler _previewHandler;

        private const string PreviewHandlerGuid = "{8895b1c6-b41f-4c1c-a562-0d564250836f}";

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
            int pcchOut = 0;

            NativeMethods.AssocQueryString(ASSOCF.INIT_DEFAULTTOSTAR, ASSOCSTR.SHELLEXTENSION, Path.GetExtension(fileName), PreviewHandlerGuid, null, ref pcchOut);

            if (pcchOut == 0)
            {
                return Guid.Empty;
            }

            var pszOut = new StringBuilder(pcchOut);

            NativeMethods.AssocQueryString(ASSOCF.INIT_DEFAULTTOSTAR, ASSOCSTR.SHELLEXTENSION, Path.GetExtension(fileName), PreviewHandlerGuid, pszOut, ref pcchOut);

            return new Guid(pszOut.ToString());
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

            if (type == null)
            {
                return null;
            }

            var previewHandler = (IPreviewHandler)Activator.CreateInstance(type);

            if (previewHandler == null)
            {
                return null;
            }

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

            try
            {
                previewHandler.Unload();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                Marshal.FinalReleaseComObject(previewHandler);
            }

            return null;
        }

        private void UnloadPreviewHandler()
        {
            if (_previewHandler != null)
            {
                try
                {
                    _previewHandler.Unload();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally
                {
                    Marshal.FinalReleaseComObject(_previewHandler);

                    _previewHandler = null;
                }
            }
        }
    }
}
