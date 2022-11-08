using System.Net.Sockets;
using System.Text;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc.Chatbox;
public static class OscChatbox
{
    public static string InputAddress = "/chatbox/input";
    public static string TypingAddress = "/chatbox/typing";
    public static void SendMessage(string message, bool direct)
    {
        OscClient client = OscUtility.Client;
        OscWriter writer = client.Writer;
        var socket = client.Socket;

        writer.Reset();
        writer.Write(InputAddress);
        writer.Write(direct ? ",sT" : ",sF");
        writer.WriteUtfString(message);
        socket.Send(writer.Buffer, writer.Length, SocketFlags.None);
    }

    public static void SetIsTyping(bool isTyping)
    {
        OscParameter.SendValue(TypingAddress, isTyping);
    }

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
