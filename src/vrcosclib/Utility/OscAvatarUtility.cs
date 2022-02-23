using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public class OscAvatarUtility
{
    public static OscAvatar CurrentAvatar => _currentAvatar;

    private static OscAvatar _currentAvatar;
    private static OscAvatar _changedAvatar;

    public static event OscValueChangedEventHandler<OscAvatar, OscAvatar>? OnAvatarChanged;

    internal static void Initialize()
    {
    }

    static OscAvatarUtility()
    {
        var server = OscUtility.Server;
        server.TryAddMethod("/avatar/change", ReadAvatarIdFromApp);
    }

    public static void ReadAvatarIdFromApp(OscMessageValues message)
    {
        _changedAvatar.AvatarId = message.ReadStringElement(message.ElementCount - 1)!;
        CallOnAvatarChanged();
    }

    private static void CallOnAvatarChanged()
    {
        var oldAvatar = _currentAvatar;
        var newAvatar = _changedAvatar;

        _currentAvatar = newAvatar;
        _changedAvatar = default;

        OnAvatarChanged?.Invoke(_currentAvatar, new ValueChangedEventArgs<OscAvatar>(oldAvatar, newAvatar));
    }
}
