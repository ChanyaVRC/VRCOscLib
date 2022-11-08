using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildSoft.VRChat.Osc.Test;

public record class OscAvatarParameterInterfaceJson
{
    public string address;

    [JsonConverter(typeof(StringEnumConverter))]
    public OscType type;
    public OscAvatarParameterInterfaceJson(string address, OscType type)
    {
        this.address = address;
        this.type = type;
    }
}
