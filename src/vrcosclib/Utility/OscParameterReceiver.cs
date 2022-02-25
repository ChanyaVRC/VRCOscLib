using System;
using System.Collections.Generic;
using System.Text;
using BlobHandles;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static class OscParameterReceiver
{
    static OscParameterReceiver()
    {
        OscUtility.Server.AddMonitorCallback(ReceiveMessage);
    }

    internal static void Initialize()
    {

    }

    internal static void ReceiveMessage(BlobString address, OscMessageValues values)
    {
        var addressString = address.ToString();
        for (int i = 0; i < values.ElementCount; i++)
        {
            OscParameter.Parameters[addressString] = values.ReadValue(i);
        }
    }
}
