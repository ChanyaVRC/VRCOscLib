namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Provides data for value change events.
/// </summary>
/// <typeparam name="T">The type of the value that was changed.</typeparam>
public class ValueChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Gets the old value.
    /// </summary>
    public T OldValue { get; }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public T NewValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    public ValueChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
