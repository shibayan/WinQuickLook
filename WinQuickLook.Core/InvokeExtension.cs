using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace WinQuickLook;

[MarkupExtensionReturnType(typeof(ICommand))]
public class InvokeExtension : MarkupExtension
{
    public InvokeExtension(string methodName)
    {
        ArgumentNullException.ThrowIfNull(methodName);

        _methodName = methodName;
    }

    private readonly string _methodName;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget provideValueTarget)
        {
            throw new InvalidOperationException();
        }

        var targetObject = (FrameworkElement)provideValueTarget.TargetObject;

        var methodInfo = targetObject.DataContext.GetType().GetMethod(_methodName);

        if (methodInfo is null)
        {
            throw new InvalidOperationException();
        }

        var parameters = methodInfo.GetParameters();

        if (parameters.Length > 1)
        {
            throw new InvalidOperationException();
        }

        return new InvokeMethodCommand(targetObject.DataContext, methodInfo, parameters.Length == 0);
    }

    private class InvokeMethodCommand : ICommand
    {
        public InvokeMethodCommand(object obj, MethodInfo methodInfo, bool parameterless)
        {
            _obj = obj;
            _methodInfo = methodInfo;
            _parameterless = parameterless;
        }

        private readonly object _obj;
        private readonly MethodInfo _methodInfo;
        private readonly bool _parameterless;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _methodInfo.Invoke(_obj, _parameterless ? null : new[] { parameter });

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
