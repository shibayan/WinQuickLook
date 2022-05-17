﻿using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Windows.Win32;

using WinQuickLook.Handlers;
using WinQuickLook.Messaging;
using WinQuickLook.Shell;

namespace WinQuickLook.App;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        PInvoke.MFStartup(PInvoke.MF_VERSION, 1);

        var services = new ServiceCollection();

        ConfigureService(services);

        using (var provider = services.BuildServiceProvider())
        {
            var app = provider.GetRequiredService<App>();

            app.Run();
        }

        PInvoke.MFShutdown();
    }

    private static void ConfigureService(IServiceCollection services)
    {
        services.TryAddEnumerable(new[]
        {
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, CodeFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, GenericDirectoryPreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, GenericFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, HtmlFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, ImageFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, MediaFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, PdfFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, ShellFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, SvgFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, TextFilePreviewHandler>()
        });

        services.AddSingleton<AssociationResolver>();
        services.AddSingleton<PreviewImageFactory>();
        services.AddSingleton<PropertyStore>();
        services.AddSingleton<ShellExplorer>();

        services.AddSingleton<KeyboardHook>();
        services.AddSingleton<MouseHook>();

        services.AddSingleton<MainWindow>();

        services.AddSingleton<App>();
    }
}
