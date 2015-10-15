using System.IO;
using System.Reflection;

using Microsoft.Win32;

namespace WinQuickLook
{
    public static class WebBrowserHelper
    {
        private const string FeatureBrowserEmulationKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

        public static void SetDocumentMode(int mode)
        {
            var fileName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            
            Registry.SetValue(FeatureBrowserEmulationKey, fileName, mode, RegistryValueKind.DWord);
        }
    }
}
