using System.Net.Sockets;
using System.Text;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc.Chatbox;

/// <summary>
/// Provides methods for sending messages and typing notifications to the VRChat chatbox.
/// </summary>
public static class OscChatbox
{
    /// <summary>
    /// The OSC address for sending chatbox input messages.
    /// </summary>
    public static string InputAddress = "/chatbox/input";

    /// <summary>
    /// The OSC address for sending typing notifications.
    /// </summary>
    public static string TypingAddress = "/chatbox/typing";

    /// <summary>
    /// Sends a message to the VRChat chatbox.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="direct">Indicates whether the message shows in direct or UI.</param>
    /// <param name="complete">Indicates whether the message uses to trigger the notification SFX.</param>
    public static void SendMessage(string message, bool direct, bool complete = false)
    {
        OscClient client = OscUtility.Client;
        OscWriter writer = client.Writer;
        var socket = client.Socket;

        writer.Reset();
        writer.Write(InputAddress);
        writer.Write((direct ? ",sT" : ",sF") + (complete ? "T" : "F"));
        writer.WriteUtfString(message);
        socket.Send(writer.Buffer, writer.Length, SocketFlags.None);
    }

    /// <summary>
    /// Sends a typing notification to the VRChat chatbox.
    /// </summary>
    /// <param name="isTyping">Indicates whether the user is typing or not typing.</param>
    public static void SetIsTyping(bool isTyping)
    {
        OscParameter.SendValue(TypingAddress, isTyping);
    }

    /// <summary>
    /// Writes the specified string in UTF-8 encoding to the OSC message.
    /// </summary>
    /// <param name="writer">The OSC message writer to write the string to.</param>
    /// <param name="data">The string to write.</param>
    private static void WriteUtfString(this OscWriter writer, string data)
    {
        var utf8String = Encoding.UTF8.GetBytes(data);
        Array.Resize(ref utf8String, utf8String.Length + (4 - utf8String.Length % 4));

        for (int i = 3; i < utf8String.Length; i += 4)
        {
            int data1 =
                utf8String[i - 3] << 8 * 3
                | utf8String[i - 2] << 8 * 2
                | utf8String[i - 1] << 8 * 1
                | utf8String[i - 0] << 8 * 0;
            writer.Write(data1);
        }
    }
}
