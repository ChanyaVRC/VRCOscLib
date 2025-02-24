using System.Diagnostics;
using Newtonsoft.Json;

namespace BuildSoft.VRChat.Osc.Avatar;

/// <summary>
/// The model of an avatar parameter.
/// </summary>
[JsonObject]
public record class OscAvatarParameter
{
#pragma warning disable IDE0044 // Add readonly modifier
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    [JsonProperty("name", Required = Required.Always)]
    private string _name = string.Empty;

    /// <summary>
    /// Gets the input interface for the parameter.
    /// </summary>
    [JsonProperty("input", Required = Required.DisallowNull)]
    private OscAvatarParameterInterface? _input;

    /// <summary>
    /// Gets the output interface for the parameter.
    /// </summary>
    [JsonProperty("output", Required = Required.DisallowNull)]
    private OscAvatarParameterInterface? _output;
#pragma warning restore IDE0044 // Add readonly modifier

    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the input interface for the parameter.
    /// </summary>
    public OscAvatarParameterInterface? Input => _input;

    /// <summary>
    /// Gets the output interface for the parameter.
    /// </summary>
    public OscAvatarParameterInterface? Output => _output;

    /// <summary>
    /// Gets the readable address for the parameter.
    /// </summary>
    public string ReadableAddress => (Output ?? Input)!.Address;

    /// <summary>
    /// Initializes a new instance of the <see cref="OscAvatarParameter"/> class.
    /// </summary>
    [JsonConstructor]
    private OscAvatarParameter()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OscAvatarParameter"/> class.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="input">The input interface for the parameter.</param>
    /// <param name="output">The output interface for the parameter.</param>
    public OscAvatarParameter(string name, OscAvatarParameterInterface? input = null, OscAvatarParameterInterface? output = null)
    {
        if (input == null && output == null)
        {
            throw new ArgumentException();
        }
        _name = name;
        _input = input;
        _output = output;
    }

    /// <summary>
    /// Create a new instance of the <see cref="OscAvatarParameter"/> class.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="type">The type of the parameter.</param>
    public static OscAvatarParameter Create(string name, OscType type)
    {
        var @interface = new OscAvatarParameterInterface(OscConst.AvatarParameterAddressSpace + name, type);
        return new(name, @interface, @interface);
    }

    private OscAvatarParameterChangedEventHandler? _valueChanged;

    /// <summary>
    /// Occurs when the value of an avatar parameter contained in this instance changes.
    /// </summary>
    public event OscAvatarParameterChangedEventHandler ValueChanged
    {
        add
        {
            if (_valueChanged == null)
            {
                OscParameter.Parameters.AddValueChangedEventByAddress(ReadableAddress, CallValueChanged);
            }
            _valueChanged += value;
        }
        remove
        {
            if (_valueChanged == null)
            {
                return;
            }

            _valueChanged -= value;
            if (_valueChanged == null)
            {
                OscParameter.Parameters.RemoveValueChangedEventByAddress(ReadableAddress, CallValueChanged);
            }
        }
    }

    private void CallValueChanged(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
    {
        Debug.Assert(_valueChanged != null);
        _valueChanged!.Invoke(this, e);
    }
}
