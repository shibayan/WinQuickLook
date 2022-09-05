using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinQuickLook.App;

public class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
