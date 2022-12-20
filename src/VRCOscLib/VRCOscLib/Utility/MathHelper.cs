using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc.Utility;

/// <summary>
/// Provide various math utility methods.
/// </summary>
internal static class MathHelper
{
    /// <summary>
    /// Clamps a value between a minimum and a maximum.
    /// </summary>
    /// <typeparam name="T">The type of the value to clamp.</typeparam>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (min.CompareTo(max) > 0)
        {
            throw new ArgumentException();
        }
        if (value.CompareTo(min) < 0)
        {
            return min;
        }
        if (value.CompareTo(max) > 0)
        {
            return max;
        }
        return value;
    }
}
