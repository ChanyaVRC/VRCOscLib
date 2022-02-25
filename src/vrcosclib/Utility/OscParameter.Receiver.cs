using System;
using System.Collections.Generic;
using System.Text;
using BlobHandles;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static partial class OscParameter
{
    internal static void ReceiveMessage(BlobString address, OscMessageValues values)
    {
        var addressString = address.ToString();
        for (int i = 0; i < values.ElementCount; i++)
        {
            Parameters[addressString] = values.ReadValue(i);
        }
    }

    public static object? GetValue(string address)
    {
        Parameters.TryGetValue(address, out var value);
        return value;
    }
}
