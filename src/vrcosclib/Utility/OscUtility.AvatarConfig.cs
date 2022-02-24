using System.Collections.Immutable;

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
            await Task.Delay(1);
            token.ThrowIfCancellationRequested();
            avatarId = OscAvatarUtility.CurrentAvatar.AvatarId;
        }
        return GetOscAvatarConfigPath(avatarId);
    }

    public static string GetOscAvatarConfigPath(string avatarId)
    {
        return Directory.EnumerateFiles(VRChatOscPath, $"*_{avatarId}.json", SearchOption.AllDirectories).First();
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
