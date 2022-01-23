using System;

namespace WinQuickLook.CsWin32;

// ReSharper disable once InconsistentNaming
public static class CLSID
{
    public static readonly Guid ShellWindows = new("9BA05972-F6A8-11CF-A442-00A0C90A8F39");
    public static readonly Guid ShellLink = new("00021401-0000-0000-C000-000000000046");
    public static readonly Guid CMultiLanguage = new("275c23e2-3747-11d0-9fea-00aa003f8646");

    public static readonly Type ShellWindowsType = Type.GetTypeFromCLSID(ShellWindows)!;
    public static readonly Type ShellLinkType = Type.GetTypeFromCLSID(ShellLink)!;
    public static readonly Type CMultiLanguageType = Type.GetTypeFromCLSID(CMultiLanguage)!;
}
