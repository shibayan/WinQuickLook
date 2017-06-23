using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace WinQuickLook.Interop
{
    public interface IUnknown
    {
    }

    [ComImport]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItem : IUnknown
    {
        void BindToHandler([In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        void GetParent([Out] out IShellItem ppsi);

        void GetDisplayName([In] SIGDN sigdnName, [Out] out IntPtr ppszName);

        void GetAttributes([In] uint sfgaoMask, [Out] out uint psfgaoAttribs);

        void Compare([In] IShellItem psi, [In] uint hint, [Out] out int piOrder);
    }

    [ComImport]
    [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItemImageFactory : IUnknown
    {
        void GetImage([In, MarshalAs(UnmanagedType.Struct)] SIZE size, [In] SIIGBF flags, [Out] out IntPtr phbm);
    }

    [ComImport]
    [Guid("85CB6900-4D95-11CF-960C-0080C7F4EE85")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IShellWindows : IUnknown
    {
        int Count { get; }

        [return: MarshalAs(UnmanagedType.IDispatch)]
        object Item([In, Optional, MarshalAs(UnmanagedType.Struct)] object index);

        [return: MarshalAs(UnmanagedType.IUnknown)]
        object _NewEnum();

        void Register([In, MarshalAs(UnmanagedType.IDispatch)] object pid, [In] IntPtr hwnd, [In] int swClass, [Out] out int plCookie);

        void RegisterPending([In] int lThreadId, [In] object pvarloc, [In] object pvarlocRoot, [In] int swClass, [Out] out int plCookie);

        void Revoke(int lCookie);

        void OnNavigate([In] int lCookie, [In] object pvarLoc);

        void OnActivated([In] int lCookie, [In, MarshalAs(UnmanagedType.VariantBool)] bool fActive);

        void FindWindowSW([In] ref object pvarLoc, [In] ref object pvarLocRoot, [In] ShellWindowTypeConstants swClass, [Out] out IntPtr phwnd, [In] ShellWindowFindWindowOptions swfwOptions, [Out, MarshalAs(UnmanagedType.Interface)] out IWebBrowserApp ppdispOut);

        void OnCreated([In] long lCookie, [In, MarshalAs(UnmanagedType.IUnknown)] object punk);

        void ProcessAttachDetach([In, MarshalAs(UnmanagedType.VariantBool)] bool fAttach);
    }

    [ComImport]
    [Guid("0002DF05-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IWebBrowserApp : IUnknown
    {
        void GoBack();

        void GoForward();

        void GoHome();

        void GoSearch();

        void Navigate([In] string url, [In, Optional] object flags, [In, Optional] object targetFrameName, [In, Optional] object postData, [In, Optional] object headers);

        void Refresh();

        void Refresh2([In, Optional] object level);

        void Stop();

        [return: MarshalAs(UnmanagedType.IDispatch)]
        object get_Application();

        [return: MarshalAs(UnmanagedType.IDispatch)]
        void get_Parent();

        [return: MarshalAs(UnmanagedType.IDispatch)]
        void get_Container();

        [return: MarshalAs(UnmanagedType.IDispatch)]
        void get_Document();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool get_TopLevelContainer();

        string get_Type();

        int get_Left();

        void put_Left([In] int left);

        int get_Top();

        void put_Top([In] int top);

        int get_Width();

        void put_Width([In] int width);

        int get_Height();

        void put_Height([In] int height);

        string get_LocationName();

        string get_LocationURL();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool get_Busy();

        // IWebBrowserApp
        void Quit();

        void ClientToWindow([In, Out] ref int pcx, [In, Out] ref int pcy);

        void PutProperty([In] string property, [In] object vtValue);

        object GetProperty([In] string property);

        string get_Name();

        IntPtr get_HWND();

        string get_FullName();

        string get_Path();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool get_Visible();

        void put_Visible([In, MarshalAs(UnmanagedType.VariantBool)] bool value);

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool get_StatusBar();

        void put_StatusBar([In, MarshalAs(UnmanagedType.VariantBool)] bool value);

        string get_StatusText();

        void put_StatusText([In] string statusText);

        int get_ToolBar();

        void put_ToolBar([In] int value);

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool get_MenuBar();

        void put_MenuBar([In, MarshalAs(UnmanagedType.VariantBool)] bool value);

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool get_FullScreen();

        void put_FullScreen([In, MarshalAs(UnmanagedType.VariantBool)] bool bFullScreen);
    }

    [ComImport]
    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider : IUnknown
    {
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.I4)]
        int QueryService([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppvObject);
    }

    [ComImport]
    [Guid("000214E2-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellBrowser : IUnknown
    {
        void GetWindow([Out] IntPtr phwnd);

        void ContextSensitiveHelp([In] bool fEnterMode);

        void InsertMenusSB([In] IntPtr hmenuShared, [In, Out] IntPtr lpMenuWidths);

        void SetMenuSB([In] IntPtr hmenuShared, [In] IntPtr holemenuRes, [In] IntPtr hwndActiveObject);

        void RemoveMenusSB([In] IntPtr hmenuShared);

        void SetStatusTextSB([In, MarshalAs(UnmanagedType.LPWStr)] string pszStatusText);

        void EnableModelessSB([In] bool fEnable);

        void TranslateAcceleratorSB([In] IntPtr pmsg, [In] ushort wId);

        void BrowseObject([In] IntPtr pidl, [In] uint wFlags);

        void GetViewStateStream([In] uint grfMode, [Out] out IntPtr ppStrm);

        void GetControlWindow([In] uint id, [Out] IntPtr phwnd);

        void SendControlMsg([In] uint id, [In] uint uMsg, [In] uint wParam, [In] uint lParam, [Out] IntPtr pret);

        void QueryActiveShellView([Out] out IShellView ppshv);

        void OnViewWindowActive([In] IShellView pshv);

        void SetToolbarItems([In] IntPtr lpButtons, [In] uint nButtons, [In] uint uFlags);
    }

    [ComImport]
    [Guid("000214E3-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellView : IUnknown
    {
    }

    [ComImport]
    [Guid("cde725b0-ccc9-4519-917e-325d72fab4ce")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFolderView : IUnknown
    {
        void GetCurrentViewMode([Out] out uint pViewMode);

        void SetCurrentViewMode([In] uint viewMode);

        void GetFolder([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        void Item([In] int iItemIndex, [Out] out IntPtr ppidl);

        void ItemCount([In] uint uFlags, [Out] out int pcItems);

        void Items([In] uint uFlags, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        void GetSelectionMarkedItem([Out] out int piItem);

        void GetFocusedItem([Out] out int piItem);

        void GetItemPosition([In] IntPtr pidl, [Out] out POINT ppt);

        void GetSpacing([In, Out] ref POINT ppt);

        void GetDefaultSpacing([Out] out POINT ppt);

        void GetAutoArrange();

        void SelectItem([In] int iItem, [In] uint dwFlags);

        void SelectAndPositionItems([In] uint cidl, [In] IntPtr apidl, [In] IntPtr apt, [In] uint dwFlags);
    }

    [ComImport]
    [Guid("1AC3D9F0-175C-11d1-95BE-00609797EA4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistFolder2 : IUnknown
    {
        void GetClassID([Out] IntPtr pClassId);

        void Initialize([In] IntPtr pidl);

        void GetCurFolder([Out] IntPtr ppidl);
    }

    [ComImport]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellFolder : IUnknown
    {
        void ParseDisplayName([In] IntPtr hwnd, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [In, Out] ref uint pchEaten, [Out] out IntPtr ppidl, [In, Out] ref uint pdwAttributes);

        void EnumObjects([In] IntPtr hwnd, [In] uint grfFlags, [Out] IntPtr ppenumIdList);

        void BindToObject([In] IntPtr pidl, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        void BindToStorage([In] IntPtr pidl, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        void CompareIDs([In] uint lParam, [In] IntPtr pidl1, [In] IntPtr pidl2);

        void CreateViewObject([In] IntPtr hwndOwner, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        void GetAttributesOf([In] uint cidl, [In] IntPtr apidl, [In, Out] ref IntPtr rgfInOut);

        void GetUIObjectOf([In] IntPtr hwndOwner, [In] uint cidl, [In] IntPtr apidl, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [In, Out] ref uint rgfReserved, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        void GetDisplayNameOf([In] IntPtr pidl, [In] SHGDNF uFlags, [Out] out STRRET pName);

        void SetNameOf([In] IntPtr hwnd, [In] IntPtr pidl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszName, [In] uint uFlags, [Out] out IntPtr ppidlOut);
    }

    [ComImport]
    [Guid("8895b1c6-b41f-4c1c-a562-0d564250836f")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPreviewHandler : IUnknown
    {
        void SetWindow(IntPtr hwnd, ref Rectangle rect);

        void SetRect(ref Rectangle rect);

        void DoPreview();

        void Unload();

        void SetFocus();

        void QueryFocus(out IntPtr phwnd);

        uint TranslateAccelerator(ref IntPtr pmsg);
    }

    [ComImport]
    [Guid("b7d14566-0509-4cce-a71f-0a554233bd9b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithFile
    {
        void Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszFilePath, uint grfMode);
    }

    [ComImport]
    [Guid("7F73BE3F-FB79-493C-A6C7-7EE14E245841")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithItem
    {
        void Initialize([In] IShellItem psi, uint grfMode);
    }

    [ComImport]
    [Guid("DCCFC164-2B38-11d2-B7EC-00C04F8F5D9A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMultiLanguage2 : IUnknown
    {
        void GetNumberOfCodePageInfo([Out] out uint pcCodePage);

        void GetCodePageInfo(uint uiCodePage, [In] ushort LangId, [Out] out IntPtr pCodePageInfo);

        void GetFamilyCodePage([In] uint uiCodePage, [Out] out uint puiFamilyCodePage);

        void EnumCodePages([In] uint grfFlags, [In] ushort LangId, [Out] out IntPtr ppEnumCodePage);

        void GetCharsetInfo(
            /* [in] */ [In, MarshalAs(UnmanagedType.BStr)] string Charset, [Out] out IntPtr pCharsetInfo);

        void IsConvertible([In] uint dwSrcEncoding, [In] uint dwDstEncoding);

        void ConvertString([In, Out] ref uint pdwMode, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] IntPtr pSrcStr, [In, Out] ref uint pcSrcSize, [Out] IntPtr pDstStr, [In, Out] ref uint pcDstSize);

        void ConvertStringToUnicode([In, Out] ref uint pdwMode, [In] uint dwEncoding, [In] IntPtr pSrcStr, [In, Out] ref uint pcSrcSize, [Out] IntPtr pDstStr, [In, Out] ref uint pcDstSize);

        void ConvertStringFromUnicode([In, Out] ref uint pdwMode, [In] uint dwEncoding, [In] IntPtr pSrcStr, [In, Out] ref uint pcSrcSize, [Out] IntPtr pDstStr, [In, Out] ref uint pcDstSize);

        void ConvertStringReset();

        void GetRfc1766FromLcid([In] uint Locale, [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrRfc1766);

        void GetLcidFromRfc1766([Out] out uint pLocale, [In, MarshalAs(UnmanagedType.BStr)] string bstrRfc1766);

        void EnumRfc1766([In] ushort LangId, [Out] out IntPtr ppEnumRfc1766);

        void GetRfc1766Info([In] uint Locale, [In] ushort LangId, [Out] IntPtr pRfc1766Info);

        void CreateConvertCharset([In] uint uiSrcCodePage, [In] uint uiDstCodePage, [In] uint dwProperty, [Out] out IntPtr ppMLangConvertCharset);

        void ConvertStringInIStream([In, Out] ref uint pdwMode, [In] uint dwFlag, [In] IntPtr lpFallBack, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] IntPtr pstmIn, [In] IntPtr pstmOut);

        void ConvertStringToUnicodeEx([In, Out] ref uint pdwMode, [In] uint dwEncoding, [In] IntPtr pSrcStr, [In, Out] ref uint pcSrcSize, [Out] IntPtr pDstStr, [In, Out] ref uint pcDstSize, [In] uint dwFlag, [In] IntPtr lpFallBack);

        void ConvertStringFromUnicodeEx([In, Out] ref uint pdwMode, [In] uint dwEncoding, [In] IntPtr pSrcStr, [In, Out] ref uint pcSrcSize, [Out] IntPtr pDstStr, [In, Out] ref uint pcDstSize, [In] uint dwFlag, [In] IntPtr lpFallBack);

        void DetectCodepageInIStream([In] uint dwFlag, [In] uint dwPrefWinCodePage, [In] IntPtr pstmIn, [Out] out DetectEncodingInfo lpEncoding, [In, Out] ref int pnScores);

        void DetectInputCodepage([In] uint dwFlag, [In] uint dwPrefWinCodePage, [In] IntPtr pSrcStr, [In, Out] ref int pcSrcSize, [Out] out DetectEncodingInfo lpEncoding, [In, Out] ref int pnScores);

        void ValidateCodePage([In] uint uiCodePage, [In] IntPtr hwnd);

        void GetCodePageDescription([In] uint uiCodePage, [In] uint lcid, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpWideCharStr, [In] int cchWideChar);

        void IsCodePageInstallable([In] uint uiCodePage);

        void SetMimeDBSource([In] IntPtr dwSource);

        void GetNumberOfScripts([Out] out uint pnScripts);

        void EnumScripts([In] uint dwFlags, [In] ushort LangId, [Out] out IntPtr ppEnumScript);

        void ValidateCodePageEx([In] uint uiCodePage, [In] IntPtr hwnd, [In] uint dwfIODControl);
    }
}
