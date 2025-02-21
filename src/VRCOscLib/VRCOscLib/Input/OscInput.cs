using System.Reflection;
using BuildSoft.VRChat.Osc.Utility;

namespace BuildSoft.VRChat.Osc.Input;

/// <summary>
/// Provides a way to manipulate input controls with OSC.
/// </summary>
public static class OscInput
{
    /// <summary>
    /// A cache of OSC addresses for <see cref="OscAxisInput"/> values.
    /// </summary>
    private static readonly Dictionary<OscAxisInput, string> _axisInputAddressCache = [];

    /// <summary>
    /// A cache of OSC addresses for <see cref="OscButtonInput"/> values.
    /// </summary>
    private static readonly Dictionary<OscButtonInput, string> _buttonInputAddressCache = [];

    /// <summary>
    /// The active button inputs.
    /// </summary>
    private static IEnumerable<OscButtonInput>? _activeButtonInputs;

    /// <summary>
    /// The active axis inputs.
    /// </summary>
    private static IEnumerable<OscAxisInput>? _activeAxisInputs;


    /// <summary>
    /// Sends an OSC message with the specified <see cref="OscButtonInput"/> content.
    /// </summary>
    /// <param name="content">The OSC button input content.</param>
    /// <param name="isOn">A value indicating whether the button is being pressed (<see langword="true"/>) or released (<see langword="false"/>).</param>
    public static void Send(this OscButtonInput content, bool isOn = true)
    {
        OscParameter.SendValue(content.CreateAddress(), isOn ? 1 : 0);
    }

    /// <summary>
    /// Sends an OSC message to press the specified <see cref="OscButtonInput"/>.
    /// </summary>
    /// <param name="content">The OSC button input to press.</param>
    public static void Press(this OscButtonInput content)
    {
        Send(content, true);
    }

    /// <summary>
    /// Sends an OSC message to release the specified <see cref="OscButtonInput"/>.
    /// </summary>
    /// <param name="content">The OSC button input to release.</param>
    public static void Release(this OscButtonInput content)
    {
        Send(content, false);
    }

    /// <summary>
    /// Sends the specified value to the OSC address associated with the specified <see cref="OscAxisInput"/> content.
    /// The value will be clamped between -1 and 1.
    /// </summary>
    /// <param name="content">The axis input to send the value for.</param>
    /// <param name="value">The value to send. This will be clamped between -1 and 1.</param>
    public static void Send(this OscAxisInput content, float value)
    {
        OscParameter.SendValue(content.CreateAddress(), MathHelper.Clamp(value, -1f, 1f));
    }

    /// <summary>
    /// Gets the OSC address associated with the specified <see cref="OscButtonInput"/> content.
    /// </summary>
    /// <param name="content">The button input to get the OSC address for.</param>
    /// <returns>The OSC address associated with the specified button input.</returns>
    public static string CreateAddress(this OscButtonInput content)
    {
        if (_buttonInputAddressCache.TryGetValue(content, out var address))
        {
            return address;
        }
        lock (_buttonInputAddressCache)
        {
            if (_buttonInputAddressCache.TryGetValue(content, out address))
            {
                return address;
            }
            address = "/input/" + content.ToString();
            _buttonInputAddressCache.Add(content, address);
        }
        return address;
    }

    /// <summary>
    /// Gets the OSC address associated with the specified <see cref="OscAxisInput"/> content.
    /// </summary>
    /// <param name="content">The axis input to get the OSC address for.</param>
    /// <returns>The OSC address associated with the specified axis input.</returns>
    public static string CreateAddress(this OscAxisInput content)
    {
        if (_axisInputAddressCache.TryGetValue(content, out var address))
        {
            return address;
        }
        lock (_axisInputAddressCache)
        {
            if (_axisInputAddressCache.TryGetValue(content, out address))
            {
                return address;
            }
            address = "/input/" + content.ToString();
            _axisInputAddressCache.Add(content, address);
        }
        return address;
    }

    /// <summary>
    /// Gets the active button inputs.
    /// </summary>
    public static IEnumerable<OscButtonInput> ActiveButtonInputs => _activeButtonInputs ??= CreateActiveFields<OscButtonInput>();

    /// <summary>
    /// Gets the active axis inputs.
    /// </summary>
    public static IEnumerable<OscAxisInput> ActiveAxisInputs => _activeAxisInputs ??= CreateActiveFields<OscAxisInput>();

    /// <summary>
    /// Creates a collection of active fields of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the fields to retrieve. Must be an enumeration type.</typeparam>
    /// <returns>A collection of active fields of the specified type.</returns>
    private static IEnumerable<T> CreateActiveFields<T>() where T : Enum
    {
        return [.. typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(s => !s.IsDefined(typeof(ObsoleteAttribute), true))
            .Select(s => (T)s.GetValue(null))];
    }
}
