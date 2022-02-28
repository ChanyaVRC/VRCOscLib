namespace BuildSoft.VRChat.Osc;

public class ValueChangedEventArgs : EventArgs
{
    public object? OldValue { get; }
    public object? NewValue { get; }
    public ValueChangedReason Reason { get; }

    public ValueChangedEventArgs(object? oldValue, object? newValue, ValueChangedReason reason)
    {
        OldValue = oldValue;
        NewValue = newValue;
        Reason = reason;
    }
}
