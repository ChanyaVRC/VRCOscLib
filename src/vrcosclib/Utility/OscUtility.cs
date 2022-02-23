using System;
using System.Collections.Generic;
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

    private static OscClient? _client;
    public static OscServer Server { get; } = OscServer.GetOrCreate(9001);
    public static OscClient Client => _client ??= new OscClient("127.0.0.1", 9000);

    public static void Initialize()
    {
        OscAvatarUtility.Initialize();
        OscAvatarParametorContainer.Initialize();
    }

    public static string? GetCurrentOscAvatarConfigPath()
    {
        var avatarId = OscAvatarUtility.CurrentAvatar.AvatarId;
        if (avatarId == null)
        {
            return null;
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

    internal static object? ReadValue(this OscMessageValues value, int index)
    {
        return value.GetTypeTag(index) switch
        {
            TypeTag.False => false,
            TypeTag.True => true,
            TypeTag.Infinitum => double.PositiveInfinity,
            TypeTag.Nil => null,
            TypeTag.AltTypeString or TypeTag.String => value.ReadStringElement(index),
            TypeTag.Blob => value.ReadBlobElement(index),
            TypeTag.AsciiChar32 => value.ReadAsciiCharElement(index),
            TypeTag.Float64 => value.ReadFloat64ElementUnchecked(index),
            TypeTag.Float32 => value.ReadFloatElementUnchecked(index),
            TypeTag.Int64 => value.ReadInt64ElementUnchecked(index),
            TypeTag.Int32 => value.ReadIntElementUnchecked(index),
            TypeTag.MIDI => value.ReadMidiElementUnchecked(index),
            TypeTag.Color32 => value.ReadColor32ElementUnchecked(index),
            TypeTag.TimeTag => value.ReadTimestampElementUnchecked(index),
            TypeTag.ArrayStart => throw new InvalidOperationException(),
            TypeTag.ArrayEnd => throw new InvalidOperationException(),
            _ => throw new InvalidOperationException(),
        };
    }
}
