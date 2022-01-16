using System;
using System.Windows;

namespace WinQuickLook.Extensions;

internal static class FrameworkElementExtensions
{
    public static IDisposable Init(this FrameworkElement frameworkElement) => new InitScope(frameworkElement);

    private class InitScope : IDisposable
    {
        public InitScope(FrameworkElement frameworkElement)
        {
            _frameworkElement = frameworkElement;

            _frameworkElement.BeginInit();
        }

        private readonly FrameworkElement _frameworkElement;

        public void Dispose() => _frameworkElement.EndInit();
    }
}
