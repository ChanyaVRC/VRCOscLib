namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Provides data for value change events.
/// </summary>
/// <typeparam name="T">The type of the value that was changed.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
/// </remarks>
/// <param name="oldValue">The old value.</param>
/// <param name="newValue">The new value.</param>
public class ValueChangedEventArgs<T>(T oldValue, T newValue) : EventArgs
{
    /// <summary>
    /// Gets the old value.
    /// </summary>
    public T OldValue { get; } = oldValue;

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public T NewValue { get; } = newValue;
}
