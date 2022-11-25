namespace BuildSoft.VRChat.Osc;

public class ParameterChangedEventArgs : ValueChangedEventArgs
{
    public string Address { get; }
    public ParameterChangedEventArgs(object? oldValue, object? newValue, string address, ValueChangedReason reason)
        : base(oldValue, newValue, reason)
    {
        Address = address;
    }
}
