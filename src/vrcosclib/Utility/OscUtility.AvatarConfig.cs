using System.Collections.Immutable;
using BuildSoft.VRChat.Osc.Avatar;

namespace BuildSoft.VRChat.Osc;
public static partial class OscUtility
{
    public static string? GetCurrentOscAvatarConfigPath()
    {
        var avatarId = OscAvatarUtility.CurrentAvatar.AvatarId;
        if (avatarId == null)
        {
            return null;
        }
        return GetOscAvatarConfigPath(avatarId);
    }

    public static async ValueTask<string> WaitAndGetCurrentOscAvatarConfigPathAsync(CancellationToken token = default)
    {
        string? avatarId = OscAvatarUtility.CurrentAvatar.AvatarId;
        while (avatarId == null)
        {
            await Task.Delay(1, token);
            avatarId = OscAvatarUtility.CurrentAvatar.AvatarId;
        }
        return GetOscAvatarConfigPath(avatarId);
    }

    public static string GetOscAvatarConfigPath(string avatarId)
    {
        try
        {
            return Directory.EnumerateFiles(VRChatOscPath, avatarId + ".json", SearchOption.AllDirectories).First();
        }
        catch (InvalidOperationException ex)
        {
            throw new FileNotFoundException("Current avatar config file is not found.", ex);
        }
    }

    public static ImmutableArray<string> GetOscAvatarConfigPathes()
    {
        if (!Directory.Exists(VRChatOscPath))
        {
            return ImmutableArray<string>.Empty;
        }

        try
        {
            return Directory.EnumerateDirectories(VRChatOscPath, "Avatars", SearchOption.AllDirectories)
                .SelectMany(s => Directory.EnumerateFiles(s, "*.json"))
                .ToImmutableArray();
        }
        catch (DirectoryNotFoundException)
        {
            return ImmutableArray<string>.Empty;
        }
    }
}
