using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static class OscUtility
{
    public static readonly string UserProfile = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
    public static readonly string VRChatAppDataPath = Path.Combine(UserProfile, @"AppData\LocalLow\VRChat\VRChat");
    public static readonly string VRChatOscPath = Path.Combine(VRChatAppDataPath, @"Osc");

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

    private static OscServer? _server;
    private static OscClient? _client;
    internal static OscServer Server => _server ??= OscServer.GetOrCreate(9001);
    internal static OscClient Client => _client ??= new OscClient("127.0.0.1", 9000);
}
