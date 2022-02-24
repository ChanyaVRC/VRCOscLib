﻿using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static partial class OscUtility
{
    public static readonly string UserProfile = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
    public static readonly string VRChatAppDataPath = Path.Combine(UserProfile, @"AppData\LocalLow\VRChat\VRChat");
    public static readonly string VRChatOscPath = Path.Combine(VRChatAppDataPath, @"Osc");

    public static void Initialize()
    {
        OscAvatarUtility.Initialize();
        OscAvatarParametorContainer.Initialize();
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
