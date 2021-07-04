using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Interop;

using IDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace WinQuickLook.Internal
{
    internal static class AssocHandlerHelper
    {
        public static string GetAssocName(string fileName)
        {
            var pcchOut = 0;

            NativeMethods.AssocQueryString(ASSOCF.INIT_IGNOREUNKNOWN, ASSOCSTR.FRIENDLYAPPNAME, Path.GetExtension(fileName), null, null, ref pcchOut);

            if (pcchOut == 0)
            {
                return null;
            }

            var pszOut = new StringBuilder(pcchOut);

            NativeMethods.AssocQueryString(ASSOCF.INIT_IGNOREUNKNOWN, ASSOCSTR.FRIENDLYAPPNAME, Path.GetExtension(fileName), null, pszOut, ref pcchOut);

            return pszOut.ToString().Trim();
        }

        public class AssocAppEntry
        {
            public string Name { get; set; }
            public ImageSource Icon { get; set; }
        }

        public static IList<AssocAppEntry> GetAssocAppList(string fileName)
        {
            var list = new List<AssocAppEntry>();

            NativeMethods.SHAssocEnumHandlers(Path.GetExtension(fileName), ASSOC_FILTER.RECOMMENDED, out var enumAssocHandlers);

            while (enumAssocHandlers.Next(1, out var assocHandler, out _) == 0)
            {
                if (assocHandler == null)
                {
                    break;
                }

                assocHandler.GetUIName(out var uiName);
                assocHandler.GetIconLocation(out var location, out var index);

                ImageSource icon;

                if (Path.IsPathFullyQualified(location))
                {
                    var icons = new IntPtr[1];

                    NativeMethods.ExtractIconEx(location, index, null, icons, 1);

                    icon = Imaging.CreateBitmapSourceFromHIcon(icons[0], Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                    NativeMethods.DestroyIcon(icons[0]);
                }
                else
                {
                    icon = null;
                }

                list.Add(new AssocAppEntry
                {
                    Name = uiName,
                    Icon = icon
                });

                Marshal.ReleaseComObject(assocHandler);
            }

            Marshal.ReleaseComObject(enumAssocHandlers);

            return list;
        }

        public static void Invoke(string appName, string fileName)
        {
            NativeMethods.SHAssocEnumHandlers(Path.GetExtension(fileName), ASSOC_FILTER.RECOMMENDED, out var enumAssocHandlers);

            while (enumAssocHandlers.Next(1, out var assocHandler, out _) == 0)
            {
                if (assocHandler == null)
                {
                    break;
                }

                assocHandler.GetUIName(out var uiName);

                if (appName == uiName)
                {
                    NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, typeof(IShellItem).GUID, out var shellItem);

                    shellItem.BindToHandler(IntPtr.Zero, BHID.DataObject, typeof(IDataObject).GUID, out var dataObject);

                    assocHandler.Invoke((IDataObject)dataObject);

                    Marshal.ReleaseComObject(dataObject);
                    Marshal.ReleaseComObject(shellItem);
                    Marshal.ReleaseComObject(assocHandler);

                    break;
                }

                Marshal.ReleaseComObject(assocHandler);
            }

            Marshal.ReleaseComObject(enumAssocHandlers);
        }
    }
}
