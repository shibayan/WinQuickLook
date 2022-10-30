using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

using WinQuickLook.Handlers;
using WinQuickLook.Windows;

namespace WinQuickLook.App.ViewModels;

public class MainWindowViewModel : BindableBase
{
    public MainWindowViewModel(IEnumerable<IFileSystemPreviewHandler> previewHandlers, AssociationResolver associationResolver)
    {
        _previewHandlers = previewHandlers;
        _associationResolver = associationResolver;

        CloseCommand = new DelegateCommand(Close);
        OpenWithDefaultCommand = new DelegateCommand(OpenWithDefault);
        OpenWithProgramCommand = new DelegateCommand(OpenWithProgram);
    }

    private readonly IEnumerable<IFileSystemPreviewHandler> _previewHandlers;
    private readonly AssociationResolver _associationResolver;

    public ICommand CloseCommand { get; }
    public ICommand OpenWithDefaultCommand { get; }
    public ICommand OpenWithProgramCommand { get; }

    private FrameworkElement? _viewer;

    public FrameworkElement? Viewer
    {
        get => _viewer;
        set => SetProperty(ref _viewer, value);
    }

    private FileSystemInfo? _openFileSystem;

    public FileSystemInfo? OpenFileSystem
    {
        get => _openFileSystem;
        set => SetProperty(ref _openFileSystem, value);
    }

    private string _defaultName = "";

    public string DefaultName
    {
        get => _defaultName;
        set => SetProperty(ref _defaultName, value);
    }

    private IReadOnlyList<AssociationResolver.Entry> _recommends = Array.Empty<AssociationResolver.Entry>();

    public IReadOnlyList<AssociationResolver.Entry> Recommends
    {
        get => _recommends;
        set => SetProperty(ref _recommends, value);
    }

    private void Close()
    {
        Viewer = null;
    }

    private void OpenWithDefault()
    {
    }

    private void OpenWithProgram()
    {
    }
}
