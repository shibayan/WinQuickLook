using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.UI.Shell.PropertiesSystem;

using WinQuickLook.Extensions;

namespace WinQuickLook.Shell;

public class ShellPropertyStore
{
    public void GetMusicProperties(FileInfo fileInfo)
    {
        if (PInvoke.SHGetPropertyStoreFromParsingName(fileInfo.FullName, null, GETPROPERTYSTOREFLAGS.GPS_DEFAULT, out IPropertyStore propertyStore).Failed)
        {
            return;
        }

        var title = propertyStore.GetString(PInvoke.PKEY_Title);
        var artist = propertyStore.GetStringArray(PInvoke.PKEY_Music_Artist).First();
        var album = propertyStore.GetString(PInvoke.PKEY_Music_AlbumTitle);

        Marshal.ReleaseComObject(propertyStore);
    }
}
