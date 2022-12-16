namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Provides data for value change events.
/// </summary>
public class ValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the old value.
    /// </summary>
    public object? OldValue { get; }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public object? NewValue { get; }

    /// <summary>
    /// Gets the reason for the change in value.
    /// </summary>
    public ValueChangedReason Reason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="reason">The reason for the change in the value.</param>
    public ValueChangedEventArgs(object? oldValue, object? newValue, ValueChangedReason reason)
    {
        OldValue = oldValue;
        NewValue = newValue;
        Reason = reason;
    }
}
