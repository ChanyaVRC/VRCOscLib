﻿using BlobHandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildSoft.VRChat.Osc.Avatar;

[JsonObject]
public class OscAvatarParameterInterface
{
#pragma warning disable IDE0044 // Add readonly modifier
    [JsonProperty("address", Required = Required.Always)]
    private string _address = string.Empty;
    private BlobString _addressBlob = default;
    [JsonProperty("type", Required = Required.Always, ItemConverterType = typeof(StringEnumConverter))]
    private OscType _type = 0;
#pragma warning restore IDE0044 // Add readonly modifier

    public string Address => _address;
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

    public OscType OscType => _type;
    public string Type => _type.ToString();


    [JsonConstructor]
    private OscAvatarParameterInterface()
    {

    }

    public OscAvatarParameterInterface(string address, OscType type)
    {
        _address = address;
        _type = type;
    }
}
