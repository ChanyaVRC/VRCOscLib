using System;

namespace BuildSoft.VRChat.Osc;

public class ValueChangedEventArgs : EventArgs
{
    public object? OldValue;
    public object? NewValue;

    public ValueChangedEventArgs(object? oldValue, object? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class ValueChangedEventArgs<T> : EventArgs
{
    public T OldValue;
    public T NewValue;

    public ValueChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
