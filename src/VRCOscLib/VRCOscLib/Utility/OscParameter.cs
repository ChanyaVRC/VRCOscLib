using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using BuildSoft.OscCore;
using ParamChangedHandler = BuildSoft.VRChat.Osc.OscParameterChangedEventHandler<BuildSoft.VRChat.Osc.IReadOnlyOscParameterCollection>;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Provides a way to get/set OSC parameters and parameter values.
/// </summary>
public static partial class OscParameter
{
    /// <summary>
    /// A collection of OSC parameters.
    /// </summary>
    internal static OscParameterCollection Parameters { get; } = new();

    /// <summary>
    /// An event that is raised when the value of an OSC parameter is changed.
    /// </summary>
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

    /// <summary>
    /// Initializes the OSC parameter system.
    /// </summary>
    internal static void Initialize()
    {
        MonitorCallback callback = ReceiveMessage;
        OscUtility.UnregisterMonitorCallback(callback);
        OscUtility.RegisterMonitorCallback(callback);
    }
}
