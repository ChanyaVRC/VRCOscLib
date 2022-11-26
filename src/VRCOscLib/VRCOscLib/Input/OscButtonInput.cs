using System.ComponentModel;

namespace BuildSoft.VRChat.Osc.Input;

public enum OscButtonInput
{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    LookLeft,
    LookRight,
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    LookDown,
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    LookUp,
    Jump,
    Run,
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    Back,
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    Menu,
    ComfortLeft,
    ComfortRight,
    DropRight,
    UseRight,
    GrabRight,
    DropLeft,
    UseLeft,
    GrabLeft,
    PanicButton,
    QuickMenuToggleLeft,
    QuickMenuToggleRight,
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    ToggleSitStand,
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    AFKToggle,
    Voice,
}
