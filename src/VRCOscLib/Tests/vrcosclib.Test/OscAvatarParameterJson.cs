using Newtonsoft.Json;

namespace BuildSoft.VRChat.Osc.Test;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record class OscAvatarParameterJson
{
    public string name;
    public OscAvatarParameterInterfaceJson? input;
    public OscAvatarParameterInterfaceJson? output;

    public OscAvatarParameterJson(string name, OscAvatarParameterInterfaceJson? input, OscAvatarParameterInterfaceJson? output)
    {
        this.name = name;
        this.input = input;
        this.output = output;
    }

    public OscAvatarParameterJson(string name, OscType type, bool hasInput = true)
        : this(name,
              input: hasInput ? new(OscConst.AvatarParameterAddressSpace + name, type) : null,
              output: new(OscConst.AvatarParameterAddressSpace + name, type))
    {
    }
}
