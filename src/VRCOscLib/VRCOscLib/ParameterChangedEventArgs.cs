namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Provides data for OSC parameter change events.
/// </summary>
public class ParameterChangedEventArgs : ValueChangedEventArgs
{
    /// <summary>
    /// Gets the address of the OSC parameter that was changed.
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Gets what the OSC parameter was obtained from.
    /// </summary>
    public ValueSource ValueSource { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldValue">The old value of the OSC parameter.</param>
    /// <param name="newValue">The new value of the OSC parameter.</param>
    /// <param name="address">The address of the OSC parameter.</param>
    /// <param name="reason">The reason for the change in the OSC parameter's value.</param>
    /// <param name="valueSource">Indicates what the value was obtained from.</param>
    internal ParameterChangedEventArgs(object? oldValue, object? newValue, string address, ValueChangedReason reason, ValueSource valueSource)
        : base(oldValue, newValue, reason)
    {
        Address = address;
        ValueSource = valueSource;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldValue">The old value of the OSC parameter.</param>
    /// <param name="newValue">The new value of the OSC parameter.</param>
    /// <param name="address">The address of the OSC parameter.</param>
    /// <param name="reason">The reason for the change in the OSC parameter's value.</param>
    public ParameterChangedEventArgs(object? oldValue, object? newValue, string address, ValueChangedReason reason)
        : this(oldValue, newValue, address, reason, ValueSource.Application)
    {
    }
}
