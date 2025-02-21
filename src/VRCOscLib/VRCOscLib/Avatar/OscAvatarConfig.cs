using Newtonsoft.Json;

namespace BuildSoft.VRChat.Osc.Avatar;

/// <summary>
/// Represents the configuration of a VRChat avatar, including its unique ID, name, and a list of avatar parameters.
/// </summary>
public class OscAvatarConfig
{
#pragma warning disable IDE0044 // Add readonly modifier
    /// <summary>
    /// The unique ID of the avatar.
    /// </summary>
    [JsonProperty("id", Required = Required.Always)]
    private string _id = string.Empty;

    /// <summary>
    /// The name of the avatar.
    /// </summary>
    [JsonProperty("name", Required = Required.Always)]
    private string _name = string.Empty;

    /// <summary>
    /// The hash of the avatar.
    /// </summary>
    [JsonProperty("hash", Required = Required.Always)]
    private int _hash = -1;
#pragma warning restore IDE0044 // Add readonly modifier

    /// <summary>
    /// The list of avatar parameters.
    /// </summary>
    [JsonProperty("parameters", Required = Required.Always)]
    private readonly List<OscAvatarParameter> _parametersList = [];

    /// <summary>
    /// Gets the unique ID of the avatar.
    /// </summary>
    public string Id => _id;
    /// <summary>
    /// Gets the name of the avatar.
    /// </summary>
    public string Name => _name;
    /// <summary>
    /// Gets the hash value of the avatar defined by VRChat.
    /// </summary>
    public int Hash => _hash;

    [JsonIgnore]
    private OscAvatarParameterContainer? _parameters;

    /// <summary>
    /// Gets the list of avatar parameters.
    /// </summary>
    [JsonIgnore]
    public OscAvatarParameterContainer Parameters => _parameters ??= new(_parametersList);

    /// <summary>
    /// Gets a value indicating whether the avatar parameters have been created.
    /// </summary>
    internal bool IsCreatedParameters => _parameters != null;

    /// <summary>
    /// Gets any additional information that was not explicitly defined in the <see cref="OscAvatarConfig"/> class, but was present in the avatar configuration file.
    /// </summary>
    [field: JsonExtensionData]
    public Dictionary<string, object> Extra { get; } = [];


    [JsonConstructor]
    private OscAvatarConfig()
    {
        OscAvatarUtility.RegisterAvatarConfig(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OscAvatarConfig"/> class with the specified avatar ID, name, and parameters.
    /// </summary>
    /// <param name="id">The unique ID of the avatar.</param>
    /// <param name="name">The name of the avatar.</param>
    /// <param name="parameters">The avatar parameters.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="id"/> is empty.
    /// or
    /// <paramref name="name"/> is empty.
    /// </exception>
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

    /// <summary>
    /// Changes an avatar in VRChat with <see cref="Id"/>.
    /// </summary>
    public void Change()
    {
        OscAvatarUtility.ChangeAvatar(_id);
    }

    /// <summary>
    /// Creates an array of all <see cref="OscAvatarConfig"/> instances in the current directory.
    /// </summary>
    /// <returns>An array of all <see cref="OscAvatarConfig"/> instances in the current directory.</returns>
    public static OscAvatarConfig[] CreateAll() =>
        OscUtility.EnumerateOscAvatarConfigPathes()
            .AsParallel()
            .Select(GetAvatarConfig)
            .Where(config => config != null).ToArray()!;

    /// <summary>
    /// Creates an instance of <see cref="OscAvatarConfig"/> from the currently active avatar.
    /// </summary>
    /// <returns>An instance of <see cref="OscAvatarConfig"/> from the currently active avatar, or <see langword="null"/> if no avatar is active.</returns>
    public static OscAvatarConfig? CreateAtCurrent()
    {
        var path = OscUtility.GetCurrentOscAvatarConfigPath();
        if (path == null)
        {
            return null;
        }
        return GetAvatarConfig(path);
    }

    /// <summary>
    /// Creates an instance of <see cref="OscAvatarConfig"/> with the specified ID.
    /// </summary>
    /// <param name="avatarId">The ID of the avatar to create.</param>
    /// <returns>An instance of <see cref="OscAvatarConfig"/> with the specified ID, or <see langword="null"/> if no avatar with the specified ID exists.</returns>
    public static OscAvatarConfig? Create(string avatarId)
    {
        return GetAvatarConfig(OscUtility.GetOscAvatarConfigPath(avatarId));
    }

    /// <summary>
    /// Asynchronously waits until the current avatar is received, and creates the avatar config.
    /// </summary>
    /// <returns>The <see cref="OscAvatarConfig"/> object that was deserialized.</returns>
    /// <exception cref="JsonSerializationException">Thrown if deserialization of the avatar configuration file fails.</exception>
    public static async ValueTask<OscAvatarConfig> WaitAndCreateAtCurrentAsync()
    {
        var path = await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync();
        var config = GetAvatarConfig(path) ?? throw new JsonSerializationException("Json Serialization failed.");
        return config;
    }

    /// <summary>
    /// Deserializes the avatar configuration file at the specified path.
    /// </summary>
    /// <param name="path">The path of the avatar configuration file to deserialize.</param>
    /// <returns>The <see cref="OscAvatarConfig"/> object that was deserialized, or <see langword="null"/> if deserialization failed.</returns>
    private static OscAvatarConfig? GetAvatarConfig(string path)
        => JsonConvert.DeserializeObject<OscAvatarConfig>(File.ReadAllText(path));
}
