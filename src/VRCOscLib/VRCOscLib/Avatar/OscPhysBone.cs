using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BuildSoft.VRChat.Osc.Avatar;

/// <summary>
/// Represents a VRCPhysBone in an avatar.
/// </summary>
public class OscPhysBone
{
    private readonly OscAvatarParameterContainer _parameters;

    /// <summary>
    /// Gets the name of the parameter that represents this VRCPhysBone.
    /// </summary>
    public string ParamName { get; }

    /// <summary>
    /// Gets a value indicating whether this VRCPhysBone is grabbed.
    /// </summary>
    public bool IsGrabbed => GetParameterValue<bool>(nameof(IsGrabbed));

    /// <summary>
    /// Gets the angle of this VRCPhysBone.
    /// </summary>
    public float Angle => GetParameterValue<float>(nameof(Angle));

    /// <summary>
    /// Gets the stretch of this VRCPhysBone.
    /// </summary>
    public float Stretch => GetParameterValue<float>(nameof(Stretch));

    /// <summary>
    /// Occurs when a parameter of this VRCPhysBone changes.
    /// </summary>
    public event OscAvatarParameterChangedEventHandler? ParameterChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="OscPhysBone"/> class with the specified avatar and parameter name.
    /// </summary>
    /// <param name="avatar">The avatar that the VRCPhysBone belongs to.</param>
    /// <param name="paramName">The name of the parameter that represents this VRCPhysBone.</param>
    public OscPhysBone(OscAvatarConfig avatar, string paramName)
        : this(avatar.Parameters, paramName, true, nameof(avatar))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OscPhysBone"/> class with the specified parameter container and parameter name.
    /// </summary>
    /// <param name="parameters">The parameter container that the VRCPhysBone belongs to.</param>
    /// <param name="paramName">The name of the parameter that represents this VRCPhysBone.</param>
    internal OscPhysBone(OscAvatarParameterContainer parameters, string paramName)
        : this(parameters, paramName, true, nameof(parameters))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OscPhysBone"/> class with the specified parameter container and parameter name.
    /// </summary>
    /// <param name="parameters">The parameter container that the VRCPhysBone belongs to.</param>
    /// <param name="paramName">The name of the parameter that represents this VRCPhysBone.</param>
    /// <param name="needCheck">Indicates whether to check if the specified parameter exists in the parameter container.</param>
    /// <param name="checkedParamName">The name of the parameter to include in the exception message if the check fails.</param>
    internal OscPhysBone(OscAvatarParameterContainer parameters, string paramName, bool needCheck, string checkedParamName = "parameters")
    {
        (string Name, OscType Type)[] actualParam = {
            (paramName + "_" + nameof(IsGrabbed), OscType.Bool ),
            (paramName + "_" + nameof(Angle),     OscType.Float),
            (paramName + "_" + nameof(Stretch),   OscType.Float),
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

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the specified parameter does not exist in the parameter container.
    /// </summary>
    /// <param name="parameters">The parameter container to check for the specified parameter.</param>
    /// <param name="paramName">The name of the parameter to check for.</param>
    /// <param name="actualParam">An array of tuples containing the names and types of the expected parameters.</param>
    /// <param name="checkedParamName">The name of the parameter to include in the exception message if the check fails.</param>
    private static void ThrowArgumentException_IfNotExistParameters(
        OscAvatarParameterContainer parameters,
        string paramName,
        (string Name, OscType Type)[] actualParam,
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
            OscType type = output.OscType;
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

    /// <summary>
    /// Raises the <see cref="ParameterChanged"/> event when a value in the parameter collection changes.
    /// </summary>
    /// <param name="sender">The parameter collection that raised the event.</param>
    /// <param name="e">The event data.</param>
    private void GetValueCallback(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
    {
        var name = e.Address.Substring(OscConst.AvatarParameterAddressSpace.Length);
        OnParameterChanged(_parameters.Get(name), e);
    }

    /// <summary>
    /// Raises the <see cref="ParameterChanged"/> event with the specified parameter and event data.
    /// </summary>
    /// <param name="param">The parameter that changed.</param>
    /// <param name="e">The event data.</param>
    protected internal void OnParameterChanged(OscAvatarParameter param, ValueChangedEventArgs e)
    {
        ParameterChanged?.Invoke(param, e);
    }

    /// <summary>
    /// Gets the value of the specified parameter as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the parameter value to get.</typeparam>
    /// <param name="name">The name of the parameter to get the value of.</param>
    /// <returns>The value of the parameter, or <see langword="null"/> if the parameter does not exist or is not of the specified type.</returns>
    private T? GetParameterValue<T>(string name) where T : notnull
    {
        return _parameters.GetAs<T>(ParamName + "_" + name);
    }
}
