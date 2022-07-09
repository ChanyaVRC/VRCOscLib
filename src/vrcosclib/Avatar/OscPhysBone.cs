using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc.Avatar;

public class OscPhysBone
{
    public OscAvatarConfig Avatar { get; }
    public string ParamName { get; }

    public bool IsGrabbed => GetParameterValue<bool>(nameof(IsGrabbed));
    public float Angle => GetParameterValue<float>(nameof(Angle));
    public float Stretch => GetParameterValue<float>(nameof(Stretch));

    public event OscAvatarParameterChangedEventHandler? ParameterChanged;

    public OscPhysBone(OscAvatarConfig avatar, string paramName)
    {
        string[] actualParamName = {
            paramName + "_" + nameof(IsGrabbed),
            paramName + "_" + nameof(Angle),
            paramName + "_" + nameof(Stretch),
        };
        ThrowArgumentException_IfNotExistParameters(avatar, paramName, actualParamName);

        Avatar = avatar;
        ParamName = paramName;

        var allParams = OscParameter.Parameters;
        for (int i = 0; i < actualParamName.Length; i++)
        {
            var address = OscConst.AvatarParameterAddressSpace + actualParamName[i];
            allParams.AddValueChangedEventByAddress(address, GetValueCallback);
        }
    }

    private static void ThrowArgumentException_IfNotExistParameters(OscAvatarConfig avatar, string paramName, string[] actualParamName)
    {
        int count = 0;
        foreach (var name in avatar.Parameters.Names)
        {
            for (int i = 0; i < actualParamName.Length; i++)
            {
                if (actualParamName[i] == name)
                {
                    count++;
                    break;
                }
            }
            if (count == actualParamName.Length)
            {
                break;
            }
        }
        if (count != actualParamName.Length)
        {
            throw new ArgumentException($"The avatar don't have the parameter \"{paramName}\".", nameof(avatar));
        }
    }

    private void GetValueCallback(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
    {
        var name = e.Address[OscConst.AvatarParameterAddressSpace.Length..];
        OnParameterChanged(Avatar.Parameters.Get(name), e);
    }

    protected internal void OnParameterChanged(OscAvatarParameter param, ValueChangedEventArgs e)
    {
        ParameterChanged?.Invoke(param, e);
    }


    private T? GetParameterValue<T>(string name) where T : notnull
    {
        return Avatar.Parameters.GetAs<T>(ParamName + "_" + name);
    }
}
