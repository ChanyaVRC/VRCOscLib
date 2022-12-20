using System.Collections.Immutable;
using BuildSoft.VRChat.Osc.Avatar;

namespace BuildSoft.VRChat.Osc;
public static partial class OscUtility
{
    /// <summary>
    /// Gets the file path of the configuration file for the currently selected avatar.
    /// </summary>
    /// <returns>The file path of the configuration file for the currently selected avatar, or <see langword="null"/> if no avatar is selected.</returns>
    public static string? GetCurrentOscAvatarConfigPath()
    {
        var avatarId = OscAvatarUtility.CurrentAvatar.Id;
        if (avatarId == null)
        {
            return null;
        }
        return GetOscAvatarConfigPath(avatarId);
    }

    /// <summary>
    /// Asynchronously waits for an avatar to be selected and then gets the file path of the configuration file for the currently selected avatar.
    /// </summary>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The file path of the configuration file for the currently selected avatar.</returns>
    /// <exception cref="TaskCanceledException">The operation was cancelled.</exception>
    public static async ValueTask<string> WaitAndGetCurrentOscAvatarConfigPathAsync(CancellationToken token = default)
    {
        string? avatarId = OscAvatarUtility.CurrentAvatar.Id;
        while (avatarId == null)
        {
            await Task.Delay(1, token);
            avatarId = OscAvatarUtility.CurrentAvatar.Id;
        }
        return GetOscAvatarConfigPath(avatarId);
    }

    /// <summary>
    /// Gets the file path for the avatar configuration file with the specified avatar ID.
    /// If the avatar configuration file is not found, a <see cref="FileNotFoundException"/> is thrown.
    /// </summary>
    /// <param name="avatarId">The avatar ID for the avatar configuration file to be retrieved.</param>
    /// <returns>The file path for the avatar configuration file with the specified avatar ID.</returns>
    /// <exception cref="FileNotFoundException">Throws if the avatar configuration file is not found.</exception>
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

    /// <summary>
    /// Gets the paths to all OSC avatar config files.
    /// </summary>
    /// <returns>An immutable array of strings representing the paths to the OSC avatar config files.</returns>
    public static ImmutableArray<string> GetOscAvatarConfigPathes()
    {
        return EnumerateOscAvatarConfigPathes().ToImmutableArray();
    }

    /// <summary>
    /// Enumerates the paths to all OSC avatar config files.
    /// </summary>
    /// <returns>An enumerable collection of strings representing the paths to the OSC avatar config files.</returns>
    internal static IEnumerable<string> EnumerateOscAvatarConfigPathes()
    {
        if (!Directory.Exists(VRChatOscPath))
        {
            return Enumerable.Empty<string>();
        }

        try
        {
            return Directory.EnumerateDirectories(VRChatOscPath, "Avatars", SearchOption.AllDirectories)
                .SelectMany(s => Directory.EnumerateFiles(s, "*.json"));
        }
        catch (DirectoryNotFoundException)
        {
            return Enumerable.Empty<string>();
        }
    }
}
