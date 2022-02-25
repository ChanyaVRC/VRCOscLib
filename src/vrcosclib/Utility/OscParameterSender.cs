using System;
using System.Collections.Generic;
using System.Text;
using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;

namespace BuildSoft.VRChat.Osc;

public static class OscParameterSender
{
    #region SendAvatarParameter
    public static void SendAvatarParameter(string name, float value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, int value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, bool value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, string value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, double value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, long value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, Vector2 value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, Vector3 value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, Color32 value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, MidiMessage value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, byte[] value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, char value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, value);
    }
    public static void SendAvatarParameter(string name, object value)
    {
        string address = OscConst.ParameterAddressSpace + name;
        SendValue(address, (dynamic)value);
    }
    #endregion

    #region SendValue
    public static void SendValue(string address, float value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, int value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, bool value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, string value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, double value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, long value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, Vector2 value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, Vector3 value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, Color32 value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, MidiMessage value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    public static void SendValue(string address, byte[] value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value, value.Length);
    }
    public static void SendValue(string address, char value)
    {
        OscParameter.Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    #endregion


}
