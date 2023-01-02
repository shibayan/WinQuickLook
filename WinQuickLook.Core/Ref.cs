using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinQuickLook;

public class Ref<T> : INotifyPropertyChanged
{
    public Ref(T? value = default)
    {
        Value = value;
    }

    private T? _value;

    public T? Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField(ref T? field, T? value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
