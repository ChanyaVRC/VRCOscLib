using BlobHandles;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc.Avatar;

public static class OscAvatarUtility
{
    internal static readonly Dictionary<string, object?> _commonParameters = new()
    {
        { "VRCFaceBlendV", null },
        { "VRCFaceBlendH", null },
        { "VRCEmote", null },
        { "VelocityY", null },
        { "VelocityX", null },
        { "VelocityZ", null },
        { "InStation", null },
        { "Seated", null },
        { "AFK", null },
        { "Upright", null },
        { "AngularY", null },
        { "Grounded", null },
        { "MuteSelf", null },
        { "VRMode", null },
        { "TrackingType", null },
        { "GestureRightWeight", null },
        { "GestureRight", null },
        { "GestureLeftWeight", null },
        { "GestureLeft", null },
        { "Voice", null },
        { "Viseme", null },
    };

    internal static List<WeakReference<OscAvatarConfig>> _avatarConfigs = new();
    internal static void RegisterAvaterConfig(OscAvatarConfig avatarConfig)
    {
        _avatarConfigs.Add(new WeakReference<OscAvatarConfig>(avatarConfig));
    }

    public static IReadOnlyDictionary<string, object?> CommonParameters => _commonParameters;

    public static OscAvatar CurrentAvatar => _currentAvatar;

    private static OscAvatar _currentAvatar;
    private static OscAvatar _changedAvatar;

    public static event OscValueChangedEventHandler<OscAvatar, OscAvatar>? AvatarChanged;

    internal static void Initialize()
    {
    }

    static OscAvatarUtility()
    {
        var server = OscUtility.Server;
        server.TryAddMethod(OscConst.AvatarIdAddress, ReadAvatarIdFromApp);
    }

    private static void ReadAvatarIdFromApp(OscMessageValues message)
    {
        _changedAvatar.AvatarId = message.ReadStringElement(message.ElementCount - 1)!;
        CallOnAvatarChanged();
    }

    public static bool IsCommonParameter(string paramName) => _commonParameters.ContainsKey(paramName);
    public static object? GetCommonParameterValue(string paramName) => _commonParameters[paramName];
    public static IEnumerable<object?> GetCommonParameterValues() => _commonParameters.Values;

    private static void CallOnAvatarChanged()
    {
        var oldAvatar = _currentAvatar;
        var newAvatar = _changedAvatar;

        _currentAvatar = newAvatar;
        _changedAvatar = default;

        AvatarChanged?.Invoke(_currentAvatar, new ValueChangedEventArgs<OscAvatar>(oldAvatar, newAvatar));
    }
}
