using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.UI.Shell.PropertiesSystem;

using WinQuickLook.Extensions;

namespace WinQuickLook.Shell;

public class PropertyStore
{
    public MusicProperties? GetMusicProperties(FileInfo fileInfo)
    {
        if (PInvoke.SHGetPropertyStoreFromParsingName(fileInfo.FullName, null, GETPROPERTYSTOREFLAGS.GPS_DEFAULT, out IPropertyStore propertyStore).Failed)
        {
            return null;
        }

        try
        {
            return new MusicProperties
            {
                Title = propertyStore.GetString(PInvoke.PKEY_Title),
                Artist = propertyStore.GetStringArray(PInvoke.PKEY_Music_Artist).FirstOrDefault(),
                Album = propertyStore.GetString(PInvoke.PKEY_Music_AlbumTitle)
            };
        }
        finally
        {
            Marshal.ReleaseComObject(propertyStore);
        }

    }

    public class MusicProperties
    {
        public string? Title { get; init; }
        public string? Artist { get; init; }
        public string? Album { get; init; }
    }
}
