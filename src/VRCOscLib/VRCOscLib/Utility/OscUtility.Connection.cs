using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static partial class OscUtility
{
    internal static OscServer Server => OscConnectionSettings.Server;
    internal static OscClient Client => OscConnectionSettings.Client;


    [Obsolete($"Use '{nameof(OscConnectionSettings)}.{nameof(OscConnectionSettings.ReceivePort)}'")]
    public static int ReceivePort
    {
        get => OscConnectionSettings.ReceivePort;
        set => OscConnectionSettings.ReceivePort = value;
    }

    [Obsolete($"Use '{nameof(OscConnectionSettings)}.{nameof(OscConnectionSettings.SendPort)}'")]
    public static int SendPort
    {
        get => OscConnectionSettings.SendPort;
        set => OscConnectionSettings.SendPort = value;
    }

    [Obsolete($"Use '{nameof(OscConnectionSettings)}.{nameof(OscConnectionSettings.VrcIPAddress)}'")]
    public static string VrcIPAddress
    {
        get => OscConnectionSettings.VrcIPAddress;
        set => OscConnectionSettings.VrcIPAddress = value;
    }


    public static void RegisterMonitorCallback(MonitorCallback callback)
    {
        var callbacks = OscConnectionSettings.MonitorCallbacks;
        Server.AddMonitorCallback(callback);
        callbacks.Add(callback);
    }
    public static void UnregisterMonitorCallback(MonitorCallback callback)
    {
        var callbacks = OscConnectionSettings.MonitorCallbacks;
        Server.RemoveMonitorCallback(callback);
        callbacks.Remove(callback);
    }
}
