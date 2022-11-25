using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc.Utility;
internal static class MathHelper
{
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
