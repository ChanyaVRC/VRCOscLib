using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// An enumeration of OSC types.
/// </summary>
public enum OscType
{
    /// <summary>
    /// The OSC type for a boolean value.
    /// </summary>
    Bool,

    /// <summary>
    /// The OSC type for an integer value.
    /// </summary>
    Int,

    /// <summary>
    /// The OSC type for a floating-point value.
    /// </summary>
    Float,
}
