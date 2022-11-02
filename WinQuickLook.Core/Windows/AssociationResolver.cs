using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Windows.Win32;
using Windows.Win32.UI.Shell;

using WinQuickLook.Extensions;

namespace WinQuickLook.Windows;

public class AssociationResolver
{
    public class Entry
    {
        public string Name { get; init; } = null!;
        public ImageSource? Icon { get; init; }
    }

    public bool TryGetDefault(FileInfo fileInfo, [NotNullWhen(true)] out Entry? entry)
    {
        var pcchOut = 0u;

        PInvoke.AssocQueryString(ASSOCF.ASSOCF_INIT_IGNOREUNKNOWN, ASSOCSTR.ASSOCSTR_FRIENDLYAPPNAME, fileInfo.Extension, null, null, ref pcchOut);

        if (pcchOut == 0)
        {
            entry = default;

            return false;
        }

        Span<char> pszOut = stackalloc char[(int)pcchOut];

        PInvoke.AssocQueryString(ASSOCF.ASSOCF_INIT_IGNOREUNKNOWN, ASSOCSTR.ASSOCSTR_FRIENDLYAPPNAME, fileInfo.Extension, null, pszOut, ref pcchOut);

        entry = new Entry { Name = new string(pszOut) };

        return true;
    }

    public IReadOnlyList<Entry> GetRecommends(FileInfo fileInfo)
    {
        if (PInvoke.SHAssocEnumHandlers(fileInfo.Extension, ASSOC_FILTER.ASSOC_FILTER_RECOMMENDED, out var enumAssocHandlers).Failed)
        {
            return Array.Empty<Entry>();
        }

        var recommends = new List<Entry>();

        var assocHandlers = new IAssocHandler?[1];

        while (enumAssocHandlers.Next(assocHandlers, out _).Succeeded)
        {
            var assocHandler = assocHandlers[0];

            if (assocHandler is null)
            {
                break;
            }

            assocHandler.GetUIName(out var pUiName);
            assocHandler.GetIconLocation(out var pIconLocation, out var iconIndex);

            var iconLocation = pIconLocation.ToString();

            BitmapSource? icon;

            if (Path.IsPathFullyQualified(iconLocation))
            {
                icon = GetIconFromResource(iconLocation, iconIndex);
            }
            else if (iconLocation.StartsWith("@"))
            {
                icon = GetIconFromIndirectString(iconLocation);
            }
            else
            {
                icon = null;
            }

            recommends.Add(new Entry
            {
                Name = pUiName.ToString(),
                Icon = icon
            });

            Marshal.ReleaseComObject(assocHandler);
        }

        Marshal.ReleaseComObject(enumAssocHandlers);

        recommends.Sort((x, y) => Comparer<string>.Default.Compare(x.Name, y.Name));

        return recommends;
    }

    private BitmapSource? GetIconFromResource(string path, int iconIndex)
    {
        PInvoke.ExtractIconEx(path, iconIndex, out var iconLarge, out var iconSmall, 1);

        try
        {
            return Imaging.CreateBitmapSourceFromHIcon(iconSmall.DangerousGetHandle(), Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                          .AsFreeze();
        }
        catch
        {
            return null;
        }
        finally
        {
            iconLarge.Close();
            iconSmall.Close();
        }
    }

    private BitmapSource? GetIconFromIndirectString(string path)
    {
        Span<char> pszOut = new char[512];

        if (PInvoke.SHLoadIndirectString(path, pszOut).Failed)
        {
            return null;
        }

        var iconImageLocation = new string(pszOut.TrimEnd('\0'));

        if (!File.Exists(iconImageLocation))
        {
            return null;
        }

        var bitmap = new BitmapImage();

        using (bitmap.Initialize())
        {
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.UriSource = new Uri(iconImageLocation);
        }

        return bitmap;
    }
}
