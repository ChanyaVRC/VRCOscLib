using System;
using System.Collections.Generic;
using System.Text;
using BlobHandles;

namespace BuildSoft.VRChat.Osc;

public static class OscInput
{
    public static void Send(this OscButtonInput content, bool isOn = true)
    {
        var client = OscUtility.Client;
        client.Send(content.CreateAddress(), isOn ? 1 : 0);
    }
    public static void Press(this OscButtonInput content)
    {
        Send(content, true);
    }
    public static void Release(this OscButtonInput content)
    {
        Send(content, false);
    }

    public static void Send(this OscAxisInput content, float value)
    {
        var client = OscUtility.Client;
        client.Send(content.CreateAddress(), Math.Clamp(value, -1f, 1f));
    }

    public static string CreateAddress(this OscButtonInput content)
    {
        return "/input/" + content.ToString();
    }
    public static string CreateAddress(this OscAxisInput content)
    {
        return "/input/" + content.ToString();
    }
}
