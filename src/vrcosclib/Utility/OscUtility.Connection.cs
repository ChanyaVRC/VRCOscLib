using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public static partial class OscUtility
{
    private static OscServer? _server;
    private static OscClient? _client;

    public static OscServer Server => _server ??= OscServer.GetOrCreate(_sendPort);
    public static OscClient Client => _client ??= new OscClient("127.0.0.1", _receivePort);

    private static int _sendPort = 9001;
    public static int SendPort
    {
        get => _sendPort;
        set
        {
            if (_server != null)
            {
                throw new InvalidOperationException("Server is working; cannot change port after to start.");
            }
            if (value > 65535 || value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _sendPort = value;
        }
    }

    private static int _receivePort = 9000;
    public static int ReceivePort
    {
        get => _receivePort;
        set
        {
            if (_client != null)
            {
                throw new InvalidOperationException("Client is working; cannot change port after to start.");
            }
            if (value > 65535 || value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _receivePort = value;
        }
    }
}
