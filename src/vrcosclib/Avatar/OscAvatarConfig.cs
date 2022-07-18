using Newtonsoft.Json;

namespace BuildSoft.VRChat.Osc.Avatar;

public class OscAvatarConfig
{
#pragma warning disable IDE0044 // Add readonly modifier
    [JsonProperty("id", Required = Required.Always)]
    private string _id = string.Empty;

    [JsonProperty("name", Required = Required.Always)]
    private string _name = string.Empty;
#pragma warning restore IDE0044 // Add readonly modifier

    [JsonProperty("parameters", Required = Required.Always)]
    private readonly List<OscAvatarParameter> _parametersList = new();

    public string Id => _id;
    public string Name => _name;

    [JsonIgnore]
    private OscAvatarParametorContainer? _parameters;
    [JsonIgnore]
    public OscAvatarParametorContainer Parameters => _parameters ??= new(_parametersList);

    internal bool IsCreatedParameters => _parameters != null;

    [field: JsonExtensionData]
    public Dictionary<string, object> Extra { get; } = new();


    [JsonConstructor]
    private OscAvatarConfig()
    {
        OscAvatarUtility.RegisterAvaterConfig(this);
    }

    public OscAvatarConfig(string id, string name, IEnumerable<OscAvatarParameter> parameters)
        : this()
    {
        if (id == "")
        {
            throw new ArgumentException($"{nameof(id)} can't be empty.", nameof(id));
        }
        if (name == "")
        {
            throw new ArgumentException($"{nameof(name)} can't be empty.", nameof(name));
        }

        _id = id;
        _name = name;
        _parametersList.AddRange(parameters);
    }


    public static OscAvatarConfig[] CreateAll() =>
        OscUtility.GetOscAvatarConfigPathes()
            .Select(GetAvatarConfig)
            .Where(config => config != null).ToArray()!;

    public static OscAvatarConfig? CreateAtCurrent()
    {
        var path = OscUtility.GetCurrentOscAvatarConfigPath();
        if (path == null)
        {
            return null;
        }
        return GetAvatarConfig(path);
    }

    public static OscAvatarConfig? Create(string avatarId)
    {
        return GetAvatarConfig(OscUtility.GetOscAvatarConfigPath(avatarId));
    }

    public static async ValueTask<OscAvatarConfig> WaitAndCreateAtCurrentAsync()
    {
        var path = await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync();
        var config = GetAvatarConfig(path);
        if (config == null)
        {
            throw new JsonSerializationException("Json Serialization failed.");
        }
        return config;
    }

    private static OscAvatarConfig? GetAvatarConfig(string path)
        => JsonConvert.DeserializeObject<OscAvatarConfig>(File.ReadAllText(path));
}
