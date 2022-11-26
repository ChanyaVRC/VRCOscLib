using System.ComponentModel;

namespace BuildSoft.VRChat.Osc.Input;

public enum OscAxisInput
{
    Vertical,
    Horizontal,
    LookHorizontal,
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Now not supported by VRChat. Do not use this. An unknown error may occur.")]
    LookVertical,
    UseAxisRight,
    GrabAxisRight,
    MoveHoldFB,
    SpinHoldCwCcw,
    SpinHoldUD,
    SpinHoldLR,
}
