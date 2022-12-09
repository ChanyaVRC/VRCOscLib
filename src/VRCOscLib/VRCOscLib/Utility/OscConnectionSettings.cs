using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static class OscConnectionSettings
{
    internal static bool _utilityInitialized = false;

    private static OscServer? _server;
    private static OscClient? _client;
    
    internal static OscServer Server => _server ??= new OscServer(_receivePort);
    internal static OscClient Client => _client ??= new OscClient(_vrcIPAddress, _sendPort);

    private static int _receivePort = 9001;
    public static int ReceivePort
    {
        get => _receivePort;
        set
        {
            if (value > 65535 || value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (_receivePort == value)
            {
                return;
            }
            _receivePort = value;
            if (!_utilityInitialized)
            {
                return;
            }
            if (_server == null)
            {
                return;
            }

            _server.Dispose();
            _server = new OscServer(value);

            var monitorCallbacks = MonitorCallbacks;
            for (int i = 0; i < monitorCallbacks.Count; i++)
            {
                _server.AddMonitorCallback(monitorCallbacks[i]);
            }
        }
    }

    private static int _sendPort = 9000;
    public static int SendPort
    {
        get => _sendPort;
        set
        {
            if (value > 65535 || value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (_sendPort == value)
            {
                return;
            }

            _sendPort = value;
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }

    private static string _vrcIPAddress = "127.0.0.1";

    public static string VrcIPAddress
    {
        get => _vrcIPAddress;
        set
        {
            // throw Exception if `value` can't be parsed.
            _vrcIPAddress = System.Net.IPAddress.Parse(value).ToString();

            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }

    internal static List<MonitorCallback> MonitorCallbacks { get; } = new List<MonitorCallback>();
}
