using System;
using System.Collections.Generic;
using System.Text;
using BlobHandles;
using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;

namespace BuildSoft.VRChat.Osc;

public static partial class OscParameter
{
    /// <summary>
    /// Handles an incoming OSC message by updating the corresponding OSC parameter in the <see cref="Parameters"/> collection.
    /// </summary>
    /// <param name="address">The address of the OSC message.</param>
    /// <param name="values">The values contained in the OSC message.</param>
    internal static void ReceiveMessage(BlobString address, OscMessageValues values)
    {
        var addressString = address.ToString();
        if (values.ElementCount <= 0)
        {
            return;
        }
        if (values.ElementCount == 1)
        {
            Parameters.SetValue(addressString, values.ReadValue(0), ValueSource.VRChat);
            return;
        }

        object?[] objects = new object[values.ElementCount];
        for (int i = 0; i < values.ElementCount; i++)
        {
            objects[i] = values.ReadValue(i);
        }
        Parameters.SetValue(addressString, objects, ValueSource.VRChat);
    }

    /// <summary>
    /// Gets the value of an OSC parameter as a <see cref="Vector3"/>.
    /// </summary>
    /// <param name="address">The address of the OSC parameter.</param>
    /// <returns>The value of the OSC parameter as a <see cref="Vector3"/>, or <see langword="null"/> if the value cannot be converted to a <see cref="Vector3"/>.</returns>
    internal static Vector3? GetValueAsVector3(string address)
    {
        Parameters.TryGetValue(address, out var value);
        if (value is Vector3 vector)
        {
            return vector;
        }
        if (value is not object[] array || array.Length != 3)
        {
            return null;
        }

        if (array[0] is not float x)
        {
            return null;
        }
        if (array[1] is not float y)
        {
            return null;
        }
        if (array[2] is not float z)
        {
            return null;
        }
        return new(x, y, z);
    }

    /// <summary>
    /// Gets the value of an OSC parameter.
    /// </summary>
    /// <param name="address">The address of the OSC parameter.</param>
    /// <returns>The value of the OSC parameter, or <see langword="null"/> if the parameter does not exist.</returns>
    public static object? GetValue(string address)
    {
        Parameters.TryGetValue(address, out var value);
        return value;
    }
}
