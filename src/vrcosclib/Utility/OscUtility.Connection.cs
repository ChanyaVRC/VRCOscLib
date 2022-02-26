using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static partial class OscUtility
{
    private static OscServer? _server;
    private static OscClient? _client;

    internal static OscServer Server => _server ??= OscServer.GetOrCreate(_receivePort);
    internal static OscClient Client => _client ??= new OscClient("127.0.0.1", _sendPort);

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
            _receivePort = value;

            if (_server == null)
            {
                return;
            }

            _server.Dispose();
            _server = new OscServer(value);

            if (_monitorCallbacks == null)
            {
                return;
            }
            for (int i = 0; i < _monitorCallbacks.Count; i++)
            {
                _server.AddMonitorCallback(_monitorCallbacks[i]);
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

            _sendPort = value;
            if (_client != null)
            {
                _client.Dispose();
                _client = new OscClient("127.0.0.1", value);
            }
        }
    }

    public static void RegisterMonitorCallback(MonitorCallback callback)
    {
        var callbacks = _monitorCallbacks ??= new();
        callbacks.Add(callback);
        Server.AddMonitorCallback(callback);
    }
    private static List<MonitorCallback>? _monitorCallbacks;
}
