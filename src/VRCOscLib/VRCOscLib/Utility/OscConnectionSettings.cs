using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// The settings for an OSC connection.
/// </summary>
public static class OscConnectionSettings
{
    /// <summary>
    /// Indicates whether <see cref="OscUtility"/> has been initialized.
    /// </summary>
    internal static bool _utilityInitialized = false;

    /// <summary>
    /// The OSC server used to receive messages.
    /// </summary>
    private static OscServer? _server;

    /// <summary>
    /// The OSC client used to send messages.
    /// </summary>
    private static OscClient? _client;

    /// <summary>
    /// Gets the OSC server used to receive messages.
    /// </summary>
    internal static OscServer Server => _server ??= new OscServer(_receivePort);

    /// <summary>
    /// Gets the OSC client used to send messages.
    /// </summary>
    internal static OscClient Client => _client ??= new OscClient(_vrcIPAddress, _sendPort);

    /// <summary>
    /// The port number used to receive OSC messages.
    /// </summary>
    private static int _receivePort = 9001;

    /// <summary>
    /// Gets or sets the port number used to receive OSC messages.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 0 or greater than 65535.</exception>
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

    /// <summary>
    /// The port number used to send OSC messages.
    /// </summary>
    private static int _sendPort = 9000;

    /// <summary>
    /// Gets or sets the port number used to send OSC messages.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 0 or greater than 65535.</exception>
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

    /// <summary>
    /// The IP address to VRChat client running.
    /// </summary>

    private static string _vrcIPAddress = "127.0.0.1";

    /// <summary>
    /// Gets or sets the IP address of the VRChat client to send OSC messages to.
    /// </summary>
    /// <exception cref="FormatException">Thrown when the value cannot be parsed as an IP address.</exception>
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

    /// <summary>
    /// A list of callbacks to be invoked when an OSC message is received.
    /// </summary>
    internal static List<MonitorCallback> MonitorCallbacks { get; } = new List<MonitorCallback>();
}
