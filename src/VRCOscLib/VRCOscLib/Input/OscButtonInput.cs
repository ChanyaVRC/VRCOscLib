using System.ComponentModel;

namespace BuildSoft.VRChat.Osc.Input;

/// <summary>
/// Specifies the type of OSC button input.
/// </summary>
public enum OscButtonInput
{
    /// <summary>
    /// The move forward button input.
    /// </summary>
    MoveForward,

    /// <summary>
    /// The move backward button input.
    /// </summary>
    MoveBackward,

    /// <summary>
    /// The move left button input.
    /// </summary>
    MoveLeft,

    /// <summary>
    /// The move right button input.
    /// </summary>
    MoveRight,

    /// <summary>
    /// The look left button input.
    /// </summary>
    LookLeft,

    /// <summary>
    /// The look right button input.
    /// </summary>
    LookRight,

    /// <summary>
    /// The look down button input.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    LookDown,

    /// <summary>
    /// The look up button input.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    LookUp,
    /// <summary>
    /// The jump button input.
    /// </summary>
    Jump,

    /// <summary>
    /// The run button input.
    /// </summary>
    Run,

    /// <summary>
    /// The back button input.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    Back,

    /// <summary>
    /// The menu button input.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    Menu,

    /// <summary>
    /// The comfort left button input.
    /// </summary>
    ComfortLeft,

    /// <summary>
    /// The comfort right button input.
    /// </summary>
    ComfortRight,

    /// <summary>
    /// The drop right button input.
    /// </summary>
    DropRight,

    /// <summary>
    /// The use right button input.
    /// </summary>
    UseRight,

    /// <summary>
    /// The grab right button input.
    /// </summary>
    GrabRight,

    /// <summary>
    /// The drop left button input.
    /// </summary>
    DropLeft,

    /// <summary>
    /// The use left button input.
    /// </summary>
    UseLeft,

    /// <summary>
    /// The grab left button input.
    /// </summary>
    GrabLeft,

    /// <summary>
    /// The panic button input.
    /// </summary>
    PanicButton,

    /// <summary>
    /// The quick menu toggle left button input.
    /// </summary>
    QuickMenuToggleLeft,

    /// <summary>
    /// The quick menu toggle right button input.
    /// </summary>
    QuickMenuToggleRight,

    /// <summary>
    /// The toggle sit/stand button input.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    ToggleSitStand,

    /// <summary>
    /// The AFK toggle button input.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    AFKToggle,

    /// <summary>
    /// The voice button input.
    /// </summary>
    Voice,
}
