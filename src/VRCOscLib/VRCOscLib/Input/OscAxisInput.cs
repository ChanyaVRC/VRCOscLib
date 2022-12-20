using System.ComponentModel;

namespace BuildSoft.VRChat.Osc.Input;

/// <summary>
/// Specifies the type of OSC axis input.
/// </summary>
public enum OscAxisInput
{
    /// <summary>
    /// The vertical axis input.
    /// </summary>
    Vertical,

    /// <summary>
    /// The horizontal axis input.
    /// </summary>
    Horizontal,

    /// <summary>
    /// The horizontal look axis input.
    /// </summary>
    LookHorizontal,

    /// <summary>
    /// The vertical look axis input.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    LookVertical,

    /// <summary>
    /// The right use axis input.
    /// </summary>
    UseAxisRight,

    /// <summary>
    /// The right grab axis input.
    /// </summary>
    GrabAxisRight,

    /// <summary>
    /// The forward/backward move-hold axis input.
    /// </summary>
    MoveHoldFB,

    /// <summary>
    /// The clockwise/counterclockwise spin-hold axis input.
    /// </summary>
    SpinHoldCwCcw,

    /// <summary>
    /// The up/down spin-hold axis input.
    /// </summary>
    SpinHoldUD,

    /// <summary>
    /// The left/right spin-hold axis input.
    /// </summary>
    SpinHoldLR,
}
