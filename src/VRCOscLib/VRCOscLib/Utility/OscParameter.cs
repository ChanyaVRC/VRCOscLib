using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using BuildSoft.OscCore;
using ParamChangedHandler = BuildSoft.VRChat.Osc.OscParameterChangedEventHandler<BuildSoft.VRChat.Osc.IReadOnlyOscParameterCollection>;

namespace BuildSoft.VRChat.Osc;
public static partial class OscParameter
{
    internal static OscParameterCollection Parameters { get; } = new();

    public static event ParamChangedHandler ValueChanged
    {
        add => Parameters.ValueChanged += value;
        remove => Parameters.ValueChanged -= value;
    }

    static OscParameter()
    {
        try
        {
            Initialize();
        }
        catch (Exception ex)
        {
            if (OscConnectionSettings._utilityInitialized)
            {
                throw;
            }
            OscUtility._initializationExceptions.Add(ex);
            return;
        }
    }

    internal static void Initialize()
    {
        MonitorCallback callback = ReceiveMessage;
        OscUtility.UnregisterMonitorCallback(callback);
        OscUtility.RegisterMonitorCallback(callback);
    }
}
