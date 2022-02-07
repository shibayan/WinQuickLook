using System;

namespace Windows.Win32;

// ReSharper disable once InconsistentNaming
public static class CLSID
{
    public static readonly Guid ShellWindows = new(0x9BA05972, 0xF6A8, 0x11CF, 0xA4, 0x42, 0x00, 0xA0, 0xC9, 0x0A, 0x8F, 0x39);
    public static readonly Guid CMultiLanguage = new(0x275C23E2, 0x3747, 0x11D0, 0x9F, 0xEA, 0x00, 0xAA, 0x00, 0x3F, 0x86, 0x46);

    public static readonly Type ShellWindowsType = Type.GetTypeFromCLSID(ShellWindows)!;
    public static readonly Type CMultiLanguageType = Type.GetTypeFromCLSID(CMultiLanguage)!;
}
