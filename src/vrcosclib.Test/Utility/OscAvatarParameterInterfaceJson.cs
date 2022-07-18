using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildSoft.VRChat.Osc.Test;

public record class OscAvatarParameterInterfaceJson(string address, [JsonConverter(typeof(StringEnumConverter))] OscType type);
