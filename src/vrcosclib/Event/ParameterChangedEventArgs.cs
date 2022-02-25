namespace BuildSoft.VRChat.Osc;

public class ParameterChangedEventArgs : ValueChangedEventArgs
{
    public string Address { get; }
    public ParameterChangedEventArgs(object? oldValue, object? newValue, string address)
        : base(oldValue, newValue)
    {
        Address = address;
    }
}
