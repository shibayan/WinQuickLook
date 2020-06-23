using Microsoft.Win32;

namespace WinQuickLook.Internal
{
    internal static class PlatformHelper
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private const string RegistryValueName = "AppsUseLightTheme";

        internal static WindowsTheme GetWindowsTheme()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);

            var registryValueObject = key?.GetValue(RegistryValueName);

            if (registryValueObject == null)
            {
                return WindowsTheme.Light;
            }

            int registryValue = (int)registryValueObject;

            return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
        }
    }

    internal enum WindowsTheme
    {
        Light,
        Dark
    }
}
