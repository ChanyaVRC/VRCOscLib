using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BuildSoft.VRChat.Osc.Avatar;

public class OscPhysBone
{
    private readonly OscAvatarParametorContainer _parameters;

    public string ParamName { get; }

    public bool IsGrabbed => GetParameterValue<bool>(nameof(IsGrabbed));
    public float Angle => GetParameterValue<float>(nameof(Angle));
    public float Stretch => GetParameterValue<float>(nameof(Stretch));

    public event OscAvatarParameterChangedEventHandler? ParameterChanged;

    public OscPhysBone(OscAvatarConfig avatar, string paramName)
        : this(avatar.Parameters, paramName, true, nameof(avatar))
    {
    }

    internal OscPhysBone(OscAvatarParametorContainer parameters, string paramName)
        : this(parameters, paramName, true, nameof(parameters))
    {
    }

    internal OscPhysBone(OscAvatarParametorContainer parameters, string paramName, bool needCheck, string checkedParamName = "parameters")
    {
        (string Name, string Type)[] actualParam = {
            (paramName + "_" + nameof(IsGrabbed), "Bool" ),
            (paramName + "_" + nameof(Angle),     "Float"),
            (paramName + "_" + nameof(Stretch),   "Float"),
        };

        if (needCheck)
        {
            ThrowArgumentException_IfNotExistParameters(parameters, paramName, actualParam, checkedParamName);
        }

        _parameters = parameters;
        ParamName = paramName;

        var allParams = OscParameter.Parameters;
        for (int i = 0; i < actualParam.Length; i++)
        {
            var address = OscConst.AvatarParameterAddressSpace + actualParam[i].Name;
            allParams.AddValueChangedEventByAddress(address, GetValueCallback);
        }
    }

    private static void ThrowArgumentException_IfNotExistParameters(
        OscAvatarParametorContainer parameters,
        string paramName,
        (string Name, string Type)[] actualParam,
        string checkedParamName)
    {
        int count = 0;
        foreach (var parameter in parameters.Items)
        {
            var output = parameter.Output;
            if (output == null)
            {
                continue;
            }

            string name = parameter.Name;
            string type = output.Type;
            for (int i = 0; i < actualParam.Length; i++)
            {
                if (actualParam[i].Name == name && actualParam[i].Type == type)
                {
                    count++;
                    break;
                }
            }
            if (count == actualParam.Length)
            {
                break;
            }
        }
        if (count != actualParam.Length)
        {
            throw new ArgumentException($"The avatar don't have the parameter \"{paramName}\".", checkedParamName);
        }
    }

    private void GetValueCallback(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
    {
        var name = e.Address[OscConst.AvatarParameterAddressSpace.Length..];
        OnParameterChanged(_parameters.Get(name), e);
    }

    protected internal void OnParameterChanged(OscAvatarParameter param, ValueChangedEventArgs e)
    {
        ParameterChanged?.Invoke(param, e);
    }


    private T? GetParameterValue<T>(string name) where T : notnull
    {
        return _parameters.GetAs<T>(ParamName + "_" + name);
    }
}
