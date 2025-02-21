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
    internal static readonly HashSet<string> _commonParameters =
    [
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
    ];

    private static List<WeakReference<OscAvatarConfig>>? _avatarConfigs;
    internal static List<WeakReference<OscAvatarConfig>> AvatarConfigs => _avatarConfigs ??= [];

    internal static void RegisterAvatarConfig(OscAvatarConfig avatarConfig)
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

    /// <summary>
    /// Changes an avatar in VRChat with <paramref name="id"/>.
    /// </summary>
    public static void ChangeAvatar(string id)
    {
        // If OSC is temporarily disabled or for some other reason the current avatar in VRChat
        // may differ from _currentAvatar. Therefore, when ChangeAvatar is called, /avatar/change will always be sent.
        OscParameter.SendValue(OscConst.AvatarIdAddress, id);
        CallOnAvatarChanged(_currentAvatar, new OscAvatar() { Id = id });
    }

    private static void ReadAvatarIdFromApp(IReadOnlyOscParameterCollection sender, ValueChangedEventArgs e)
    {
        CallOnAvatarChanged(_currentAvatar, new OscAvatar() { Id = (string?)e.NewValue });
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

    private static void CallOnAvatarChanged(OscAvatar oldAvatar, OscAvatar newAvatar)
    {
        // If OSC is temporarily disabled or for some other reason the current avatar in VRChat
        // may differ from _currentAvatar. Therefore, when ChangeAvatar is called, /avatar/change will always be sent.
        // However, if it is determined that the old and new avatars are the same,
        // the callback for the change will not be invoked because it would be impossible
        // to reliably obtain the 'definitely correct' old avatar.
        if (oldAvatar == newAvatar)
        {
            return;
        }
        _currentAvatar = newAvatar;
        AvatarChanged?.Invoke(newAvatar, new ValueChangedEventArgs<OscAvatar>(oldAvatar, newAvatar));
    }
}
