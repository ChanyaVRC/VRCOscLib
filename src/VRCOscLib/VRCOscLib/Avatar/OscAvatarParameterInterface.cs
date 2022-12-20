using BlobHandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildSoft.VRChat.Osc.Avatar;

/// <summary>
/// This class provides an interface for Open Sound Control (OSC) messages to avatar parameter.
/// </summary>
[JsonObject]
public class OscAvatarParameterInterface
{
#pragma warning disable IDE0044 // Add readonly modifier
    /// <summary>
    /// Gets the address of the parameter interface.
    /// </summary>
    [JsonProperty("address", Required = Required.Always)]
    private string _address = string.Empty;

    /// <summary>
    /// Gets the blob string for the address of the parameter interface.
    /// </summary>
    private BlobString _addressBlob = default;

    /// <summary>
    /// Gets the OSC type of the parameter interface.
    /// </summary>
    [JsonProperty("type", Required = Required.Always, ItemConverterType = typeof(StringEnumConverter))]
    private OscType _type = 0;
#pragma warning restore IDE0044 // Add readonly modifier

    /// <summary>
    /// Gets the address of the parameter interface.
    /// </summary>
    public string Address => _address;

    /// <summary>
    /// Gets the blob string for the address of the parameter interface.
    /// </summary>
    public BlobString AddressBlob
    {
        get
        {
            if (_addressBlob.Handle.Length == 0)
            {
                _addressBlob = new BlobString(_address);
            }
            return _addressBlob;
        }
    }

    /// <summary>
    /// Gets the OSC type of the parameter interface.
    /// </summary>
    public OscType OscType => _type;

    /// <summary>
    /// Gets the OSC type of the parameter interface as a string.
    /// </summary>
    public string Type => _type.ToString();


    /// <summary>
    /// Initializes a new instance of the <see cref="OscAvatarParameterInterface"/> class.
    /// </summary>
    [JsonConstructor]
    private OscAvatarParameterInterface()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OscAvatarParameterInterface"/> class.
    /// </summary>
    /// <param name="address">The address of the parameter interface.</param>
    /// <param name="type">The OSC type of the parameter interface.</param>
    public OscAvatarParameterInterface(string address, OscType type)
    {
        _address = address;
        _type = type;
    }
}
