using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BuildSoft.VRChat.Osc;

public class OscAvatarConfig
{
    [JsonProperty("id", Required = Required.Always)]
    private string _id = string.Empty;

    [JsonProperty("name", Required = Required.Always)]
    private string _name = string.Empty;

    [JsonProperty("parameters", Required = Required.Always)]
    private readonly List<OscAvatarParameter> _parametersList = new();

    public string Id => _id;
    public string Name => _name;

    [JsonIgnore]
    private OscAvatarParametorContainer? _paramaters;
    [JsonIgnore]
    public OscAvatarParametorContainer Parameters
        => _paramaters ??= new OscAvatarParametorContainer(_parametersList);

    [field: JsonExtensionData]
    public Dictionary<string, object> Extra { get; } = new();

    public static OscAvatarConfig[] CreateOscAvatarConfigs() =>
        OscUtility.GetOscAvatarConfigPathes()
            .Select(GetAvatarConfig)
            .Where(config => config != null).ToArray()!;

    public static OscAvatarConfig? CreateCurrentOscAvatarConfig()
    {
        var path = OscUtility.GetCurrentOscAvatarConfigPath();
        if (path == null)
        {
            return null;
        }
        return GetAvatarConfig(path);
    }

    private static OscAvatarConfig? GetAvatarConfig(string path)
        => JsonConvert.DeserializeObject<OscAvatarConfig>(File.ReadAllText(path));
}
