using System;
using System.Windows.Input;

namespace WinQuickLook.App;

public class DelegateCommand : ICommand
{
    public DelegateCommand(Action execute)
        : this(execute, () => true)
    {
    }

    public DelegateCommand(Action execute, Func<bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public bool CanExecute(object? parameter) => _canExecute();

    public void Execute(object? parameter) => _execute();

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

public class DelegateCommand<T> : ICommand
{
    public DelegateCommand(Action<T> execute)
        : this(execute, _ => true)
    {
    }

    public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    public bool CanExecute(object? parameter) => _canExecute((T)parameter!);

    public void Execute(object? parameter) => _execute((T)parameter!);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
