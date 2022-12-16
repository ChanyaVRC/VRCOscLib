namespace BuildSoft.VRChat.Osc.Avatar;

/// <summary>
/// Represents a VRChat avatar, as identified by its unique ID.
/// </summary>
public struct OscAvatar
{
    /// <summary>
    /// The unique ID of the avatar.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Converts the avatar to an <see cref="OscAvatarConfig"/> object.
    /// </summary>
    /// <returns>
    /// An <see cref="OscAvatarConfig"/> object representing the avatar,
    /// or null if the avatar does not have a valid ID.
    /// </returns>
    public OscAvatarConfig? ToConfig() => Id == null ? null : OscAvatarConfig.Create(Id);
}
