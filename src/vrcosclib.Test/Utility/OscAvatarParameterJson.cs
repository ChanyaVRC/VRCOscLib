using Newtonsoft.Json;

namespace BuildSoft.VRChat.Osc.Test;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record class OscAvatarParameterJson(string name, OscAvatarParameterInterfaceJson? input, OscAvatarParameterInterfaceJson? output)
{
    public OscAvatarParameterJson(string name, string type, bool hasInput = true)
        : this(name,
              input: hasInput ? new(OscConst.AvatarParameterAddressSpace + name, type) : null,
              output: new(OscConst.AvatarParameterAddressSpace + name, type))
    {
    }
}
