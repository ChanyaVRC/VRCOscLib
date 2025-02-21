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
    public readonly OscAvatarConfig? ToConfig() => Id == null ? null : OscAvatarConfig.Create(Id);

    /// <summary>
    /// Changes an avatar in VRChat with <see cref="Id"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="Id"/> is <see langword="null"/>.</exception>
    public readonly void Change()
    {
        if (Id == null)
        {
            throw new InvalidOperationException("Id not set.");
        }
        OscAvatarUtility.ChangeAvatar(Id);
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) => obj is OscAvatar avatar && Id == avatar.Id;

    /// <inheritdoc/>
    public override readonly int GetHashCode() => 2108858624 + EqualityComparer<string?>.Default.GetHashCode(Id);

    /// <summary>
    /// Determines whether two specified <see cref="OscAvatar"/> have the same value.
    /// </summary>
    /// <param name="left">The first <see cref="OscAvatar"/> to compare.</param>
    /// <param name="right">The second <see cref="OscAvatar"/> to compare.</param>
    /// <returns></returns>
    public static bool operator ==(OscAvatar left, OscAvatar right) => left.Id == right.Id;

    /// <summary>
    /// Determines whether two specified <see cref="OscAvatar"/> have different values.
    /// </summary>
    /// <param name="left">The first <see cref="OscAvatar"/> to compare.</param>
    /// <param name="right">The second <see cref="OscAvatar"/> to compare.</param>
    /// <returns></returns>
    public static bool operator !=(OscAvatar left, OscAvatar right) => left.Id != right.Id;
}
