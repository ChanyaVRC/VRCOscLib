using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Avatar;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Provides basic functionality about OSC.
/// </summary>
public static partial class OscUtility
{
    /// <summary>
    /// The path to the user's profile directory.
    /// </summary>
    internal static readonly string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    /// <summary>
    /// The path to the VRChat AppData directory.
    /// </summary>
    public static readonly string VRChatAppDataPath = Path.Combine(UserProfile, @"AppData", "LocalLow", "VRChat", "VRChat");

    /// <summary>
    /// The path to the VRChat OSC directory.
    /// </summary>
    public static readonly string VRChatOscPath = Path.Combine(VRChatAppDataPath, @"Osc");


    /// <summary>
    /// A list of exceptions that were thrown during the initialization of the OSC utility.
    /// </summary>
    internal static readonly List<Exception> _initializationExceptions = new();

    /// <summary>
    /// Indicates whether the OSC utility failed to initialize automatically.
    /// </summary>
    /// <returns><see langword="true"/> if the OSC utility failed to initialize automatically, otherwise <see langword="false"/>.</returns>
    public static bool IsFailedAutoInitialization => _initializationExceptions.Count > 0;

    /// <summary>
    /// Initializes the OSC utility.
    /// </summary>
    public static void Initialize()
    {
        OscAvatarUtility.Initialize();
    }

    static OscUtility()
    {
        try
        {
            Initialize();
        }
        catch (Exception ex)
        {
            _initializationExceptions.Add(ex);
        }
        OscConnectionSettings._utilityInitialized = true;
    }

    /// <summary>
    /// Reads a value from the specified index in an OSC message values object.
    /// </summary>
    /// <param name="value">The OSC message values object to read from.</param>
    /// <param name="index">The index of the value to read.</param>
    /// <returns>The value at the specified index in the OSC message values object, or null if the value is a nil type tag.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the type tag at the specified index is an array start or array end type tag.</exception>
    internal static object? ReadValue(this OscMessageValues value, int index)
    {
        return value.GetTypeTag(index) switch
        {
            TypeTag.Float32 => value.ReadFloatElementUnchecked(index),
            TypeTag.Int32 => value.ReadIntElementUnchecked(index),
            TypeTag.True => true,
            TypeTag.False => false,
            TypeTag.AltTypeString or TypeTag.String => value.ReadStringElement(index),
            TypeTag.Float64 => value.ReadFloat64ElementUnchecked(index),
            TypeTag.Int64 => value.ReadInt64ElementUnchecked(index),
            TypeTag.Blob => value.ReadBlobElement(index),
            TypeTag.Color32 => value.ReadColor32ElementUnchecked(index),
            TypeTag.MIDI => value.ReadMidiElementUnchecked(index),
            TypeTag.AsciiChar32 => value.ReadAsciiCharElement(index),
            TypeTag.TimeTag => value.ReadTimestampElementUnchecked(index),
            TypeTag.Infinitum => double.PositiveInfinity,
            TypeTag.Nil => null,
            TypeTag.ArrayStart => throw new InvalidOperationException(),
            TypeTag.ArrayEnd => throw new InvalidOperationException(),
            _ => throw new InvalidOperationException(),
        };
    }

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns><see langword="true"/> if the specified objects are equal, otherwise <see langword="false"/>.</returns>
    internal static bool AreEqual(object? left, object? right)
        => left is null ? right is null : left.Equals(right);
}
