using BlobHandles;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc.Avatar;

public static class OscAvatarUtility
{
    internal static readonly HashSet<string> _commonParameters = new()
    {
        "VRCFaceBlendV",
        "VRCFaceBlendH",
        "VRCEmote",
        "VelocityY",
        "VelocityX",
        "VelocityZ",
        "InStation",
        "Seated",
        "AFK",
        "Upright",
        "AngularY",
        "Grounded",
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

    internal static List<WeakReference<OscAvatarConfig>> _avatarConfigs = new();
    internal static void RegisterAvaterConfig(OscAvatarConfig avatarConfig)
    {
        _avatarConfigs.Add(new WeakReference<OscAvatarConfig>(avatarConfig));
    }

    public static IReadOnlyDictionary<string, object?> CommonParameters
        => _commonParameters.ToDictionary(s => s, GetCommonParameterValue);

    public static OscAvatar CurrentAvatar => _currentAvatar;

    private static OscAvatar _currentAvatar;
    private static OscAvatar _changedAvatar;

    public static event OscValueChangedEventHandler<OscAvatar, OscAvatar>? AvatarChanged;

    internal static void Initialize()
    {
    }

    static OscAvatarUtility()
    {
        var parameters = OscParameter.Parameters;
        parameters.AddValueChangedEventByAddress(OscConst.AvatarIdAddress, ReadAvatarIdFromApp);
    }

    private static void ReadAvatarIdFromApp(IReadOnlyOscParameterCollection sender, ValueChangedEventArgs e)
    {
        _changedAvatar.AvatarId = (string?)e.NewValue;
        CallOnAvatarChanged();
    }

    public static bool IsCommonParameter(string paramName) => _commonParameters.Contains(paramName);
    public static object? GetCommonParameterValue(string paramName) 
        => OscParameter.GetValue(OscConst.AvatarParameterAddressSpace + paramName);
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
