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
}
