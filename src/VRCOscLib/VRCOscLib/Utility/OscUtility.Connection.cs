using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static partial class OscUtility
{
    /// <inheritdoc cref="OscConnectionSettings.Server"/>
    internal static OscServer Server => OscConnectionSettings.Server;

    /// <inheritdoc cref="OscConnectionSettings.Client"/>
    internal static OscClient Client => OscConnectionSettings.Client;


    /// <inheritdoc cref="OscConnectionSettings.ReceivePort"/>
    [Obsolete($"Use '{nameof(OscConnectionSettings)}.{nameof(OscConnectionSettings.ReceivePort)}'")]
    public static int ReceivePort
    {
        get => OscConnectionSettings.ReceivePort;
        set => OscConnectionSettings.ReceivePort = value;
    }

    /// <inheritdoc cref="OscConnectionSettings.SendPort"/>
    [Obsolete($"Use '{nameof(OscConnectionSettings)}.{nameof(OscConnectionSettings.SendPort)}'")]
    public static int SendPort
    {
        get => OscConnectionSettings.SendPort;
        set => OscConnectionSettings.SendPort = value;
    }

    /// <inheritdoc cref="OscConnectionSettings.VrcIPAddress"/>
    [Obsolete($"Use '{nameof(OscConnectionSettings)}.{nameof(OscConnectionSettings.VrcIPAddress)}'")]
    public static string VrcIPAddress
    {
        get => OscConnectionSettings.VrcIPAddress;
        set => OscConnectionSettings.VrcIPAddress = value;
    }

    /// <summary>
    /// Registers a monitor callback with the OSC server and adds it to the list of monitor callbacks.
    /// </summary>
    /// <param name="callback">The callback to register and add to the list of monitor callbacks.</param>
    public static void RegisterMonitorCallback(MonitorCallback callback)
    {
        var callbacks = OscConnectionSettings.MonitorCallbacks;
        Server.AddMonitorCallback(callback);
        callbacks.Add(callback);
    }

    /// <summary>
    /// Unregisters a monitor callback from the OSC server and removes it from the list of monitor callbacks.
    /// </summary>
    /// <param name="callback">The callback to unregister and remove from the list of monitor callbacks.</param>
    public static void UnregisterMonitorCallback(MonitorCallback callback)
    {
        var callbacks = OscConnectionSettings.MonitorCallbacks;
        Server.RemoveMonitorCallback(callback);
        callbacks.Remove(callback);
    }
}
