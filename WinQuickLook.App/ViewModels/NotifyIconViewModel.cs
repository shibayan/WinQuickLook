using System.Windows;
using System.Windows.Input;

namespace WinQuickLook.App.ViewModels;

public class NotifyIconViewModel : BindableBase
{
    public NotifyIconViewModel()
    {
        ActivateCommand = new DelegateCommand(Activate);
        LaunchOnLoginCommand = new DelegateCommand<bool>(LaunchOnLogin);
        ExitCommand = new DelegateCommand(Exit);
    }

    public ICommand ActivateCommand { get; }

    public ICommand LaunchOnLoginCommand { get; }

    public ICommand ExitCommand { get; }

    private void Activate()
    {
        foreach (Window window in Application.Current.Windows)
        {
            window.Activate();
        }
    }

    private void LaunchOnLogin(bool isChecked)
    {
    }

    private void Exit() => Application.Current.Shutdown();
}
