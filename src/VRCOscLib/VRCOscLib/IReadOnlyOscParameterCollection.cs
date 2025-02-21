using ParamChangedHandler = BuildSoft.VRChat.Osc.OscParameterChangedEventHandler<BuildSoft.VRChat.Osc.IReadOnlyOscParameterCollection>;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Represents a read-only collection of OSC parameters.
/// </summary>
public interface IReadOnlyOscParameterCollection : IReadOnlyDictionary<string, object?>
{
    /// <summary>
    /// Occurs when a value in the collection changes.
    /// </summary>
    event ParamChangedHandler? ValueChanged;

    /// <summary>
    /// Adds an event handler to the collection that is invoked when the value of the parameter with the specified OSC address changes.
    /// </summary>
    /// <param name="address">The OSC address of the parameter to listen for value changes on.</param>
    /// <param name="handler">The event handler to add.</param>
    void AddValueChangedEventByAddress(string address, ParamChangedHandler handler);

    /// <summary>
    /// Removes an event handler to the collection that is invoked when the value of the parameter with the specified OSC address changes.
    /// </summary>
    /// <param name="address">The OSC address of the parameter to listen for value changes on.</param>
    /// <param name="handler">The event handler to remove.</param>
    bool RemoveValueChangedEventByAddress(string address, ParamChangedHandler handler);
}
