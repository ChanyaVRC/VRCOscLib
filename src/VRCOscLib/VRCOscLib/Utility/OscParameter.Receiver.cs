using System;
using System.Collections.Generic;
using System.Text;
using BlobHandles;
using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;

namespace BuildSoft.VRChat.Osc;

public static partial class OscParameter
{
    internal static void ReceiveMessage(BlobString address, OscMessageValues values)
    {
        var addressString = address.ToString();
        if (values.ElementCount <= 0)
        {
            return;
        }
        if (values.ElementCount == 1)
        {
            Parameters[addressString] = values.ReadValue(0);
            return;
        }

        object?[] objects = new object[values.ElementCount];
        for (int i = 0; i < values.ElementCount; i++)
        {
            objects[i] = values.ReadValue(i);
        }
        Parameters[addressString] = objects;
    }

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

    public static object? GetValue(string address)
    {
        Parameters.TryGetValue(address, out var value);
        return value;
    }
}
