namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Specifies the reason for a change in the value.
/// </summary>
public enum ValueChangedReason
{
    /// <summary>
    /// The value was added.
    /// </summary>
    Added,

    /// <summary>
    /// The value was removed.
    /// </summary>
    Removed,

    /// <summary>
    /// The value was substituted.
    /// </summary>
    Substituted
}
