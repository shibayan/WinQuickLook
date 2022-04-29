using System.IO;

using Windows.Win32;
using Windows.Win32.UI.Shell.PropertiesSystem;

namespace WinQuickLook.Shell;
public class ShellPropertyStore
{
    public void GetMetadata(FileInfo fileInfo)
    {
        if (PInvoke.SHGetPropertyStoreFromParsingName(fileInfo.FullName, null, GETPROPERTYSTOREFLAGS.GPS_DEFAULT, out IPropertyStore? propertyStore).Failed)
        {
            return;
        }
    }
}
