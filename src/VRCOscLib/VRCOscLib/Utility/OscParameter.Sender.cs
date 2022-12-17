using System;
using System.Collections.Generic;
using System.Text;
using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;

namespace BuildSoft.VRChat.Osc;

public static partial class OscParameter
{
    #region SendAvatarParameter
    /// <summary>
    /// Sends an OSC message with the specified <see cref="float"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, float value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="int"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, int value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }
    /// <summary>
    /// Sends an OSC message with the specified <see cref="bool"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, bool value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="string"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, string value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="double"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, double value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="long"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, long value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="Vector2"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, Vector2 value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="Vector3"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, Vector3 value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="Color32"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, Color32 value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="MidiMessage"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, MidiMessage value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="byte"/>[] value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, byte[] value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="char"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, char value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="object"/> value to the specified OSC address.
    /// </summary>
    /// <param name="name">The name of the OSC parameter.</param>
    /// <param name="value">The value of the OSC parameter.</param>
    public static void SendAvatarParameter(string name, object value)
    {
        string address = OscConst.AvatarParameterAddressSpace + name;
        SendValue(address, (dynamic)value);
    }
    #endregion

    #region SendValue
    /// <summary>
    /// Sends an OSC message with the specified <see cref="float"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, float value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="int"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, int value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="bool"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, bool value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="string"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, string value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="double"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, double value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="long"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, long value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="Vector2"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, Vector2 value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="Vector3"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, Vector3 value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="Color32"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, Color32 value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="MidiMessage"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, MidiMessage value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="byte"/>[] value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, byte[] value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value, value.Length);
    }

    /// <summary>
    /// Sends an OSC message with the specified <see cref="char"/> value to the specified OSC address.
    /// </summary>
    /// <param name="address">The OSC address to send the message to.</param>
    /// <param name="value">The value to send in the OSC message.</param>
    public static void SendValue(string address, char value)
    {
        Parameters[address] = value;
        OscUtility.Client.Send(address, value);
    }
    #endregion
}
