using System;
using BuildSoft.VRChat.Osc.Chatbox;

while (true)
{
    Console.Write("Text: ");
    string? text = Console.ReadLine();
    if (text == null)
    {
        Console.WriteLine("Cannot Send Text.");
        continue;
    }
    if (text != "")
    {
        OscChatbox.SetIsTyping(true);
        await Task.Delay(1000);
        OscChatbox.SendMessage(text, true);
        OscChatbox.SetIsTyping(false);
    }
}
