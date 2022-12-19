using BlobHandles;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc.Avatar;

/// <summary>
/// A utility class for interacting with VRChat avatars using the OSC protocol.
/// </summary>
public static class OscAvatarUtility
{
    /// <summary>
    /// A list of common avatar parameters.
    /// </summary>
    internal static readonly HashSet<string> _commonParameters = new()
    {
        "VelocityY",
        "VelocityX",
        "VelocityZ",
        "InStation",
        "Seated",
        "AFK",
        "Upright",
        "AngularY",
        "Grounded",
        "Earmuffs",
        "MuteSelf",
        "VRMode",
        "TrackingType",
        "GestureRightWeight",
        "GestureRight",
        "GestureLeftWeight",
        "GestureLeft",
        "Voice",
        "Viseme",
    };

    private static List<WeakReference<OscAvatarConfig>>? _avatarConfigs;
    internal static List<WeakReference<OscAvatarConfig>> AvatarConfigs => _avatarConfigs ??= new();

    internal static void RegisterAvaterConfig(OscAvatarConfig avatarConfig)
    {
        AvatarConfigs.Add(new WeakReference<OscAvatarConfig>(avatarConfig));
    }

    /// <summary>
    /// Gets a dictionary of common avatar parameters and their current values.
    /// </summary>
    public static IReadOnlyDictionary<string, object?> CommonParameters
        => _commonParameters.ToDictionary(s => s, GetCommonParameterValue);

    /// <summary>
    /// Enumerates common avatar parameters.
    /// </summary>
    public static IEnumerable<string> CommonParameterNames
        => _commonParameters.OrderBy(s => s);

    /// <summary>
    /// Gets the current <see cref="OscAvatar"/>.
    /// </summary>
    public static OscAvatar CurrentAvatar => _currentAvatar;

    private static OscAvatar _currentAvatar;
    private static OscAvatar _changedAvatar;


    /// <summary>
    /// Event that is raised when the current avatar changes.
    /// </summary>
    public static event OscValueChangedEventHandler<OscAvatar, OscAvatar>? AvatarChanged;

    /// <summary>
    /// Initializes the OSC avatar system.
    /// </summary>
    public static void Initialize()
    {
        OscParameter.Initialize();
        var parameters = OscParameter.Parameters;

        OscParameterChangedEventHandler<IReadOnlyOscParameterCollection> readAvatarIdFromApp = ReadAvatarIdFromApp;
        parameters.RemoveValueChangedEventByAddress(OscConst.AvatarIdAddress, readAvatarIdFromApp);
        parameters.AddValueChangedEventByAddress(OscConst.AvatarIdAddress, readAvatarIdFromApp);
    }

    static OscAvatarUtility()
    {
        try
        {
            Initialize();
        }
        catch (Exception ex)
        {
            if (OscConnectionSettings._utilityInitialized)
            {
                throw;
            }
            OscUtility._initializationExceptions.Add(ex);
            return;
        }
    }

    private static void ReadAvatarIdFromApp(IReadOnlyOscParameterCollection sender, ValueChangedEventArgs e)
    {
        _changedAvatar.Id = (string?)e.NewValue;
        CallOnAvatarChanged();
    }

    /// <summary>
    /// Determines whether the specified parameter is a common parameter for VRChat avatars.
    /// </summary>
    /// <param name="paramName">The name of the parameter to check.</param>
    /// <returns>true if the parameter is common; otherwise, <see langword="false"/>.</returns>
    public static bool IsCommonParameter(string paramName) => _commonParameters.Contains(paramName);

    /// <summary>
    /// Gets the value of the specified common parameter for the current avatar.
    /// </summary>
    /// <param name="paramName">The name of the parameter to get the value of.</param>
    /// <returns>The value of the specified parameter, or null if the parameter does not exist or is not common.</returns>
    public static object? GetCommonParameterValue(string paramName)
        => OscParameter.GetValue(OscConst.AvatarParameterAddressSpace + paramName);

    /// <summary>
    /// Gets the values of all common parameters for the current avatar.
    /// </summary>
    /// <returns>An enumerable containing the values of all common parameters for the current avatar.</returns>
    public static IEnumerable<object?> GetCommonParameterValues()
        => _commonParameters.Select(GetCommonParameterValue);

    private static void CallOnAvatarChanged()
    {
        var oldAvatar = _currentAvatar;
        var newAvatar = _changedAvatar;

        _currentAvatar = newAvatar;
        _changedAvatar = default;

        AvatarChanged?.Invoke(_currentAvatar, new ValueChangedEventArgs<OscAvatar>(oldAvatar, newAvatar));
    }
}
