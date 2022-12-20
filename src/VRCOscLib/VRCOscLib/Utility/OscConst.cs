using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// A class containing constants used in the OSC communication.
/// </summary>
internal class OscConst
{
    /// <summary>
    /// The address space for avatar parameter messages.
    /// </summary>
    public const string AvatarParameterAddressSpace = "/avatar/parameters/";

    /// <summary>
    /// The address for avatar change messages.
    /// </summary>
    public const string AvatarIdAddress = "/avatar/change";
}
