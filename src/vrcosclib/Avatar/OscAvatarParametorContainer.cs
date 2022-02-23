using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BlobHandles;
using BuildSoft.OscCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildSoft.VRChat.Osc;

public class OscAvatarParametorContainer : IReadOnlyDictionary<string, object>
{
    internal static void Initialize()
    {

    }

    public OscAvatarParametorContainer(ImmutableArray<OscAvatarParameter> parameters)
    {
        Parameters = parameters;

        var uniqueParamsBuilder = ImmutableDictionary.CreateBuilder<BlobString, OscAvatarParameter>();
        var uniqueParamValues = new Dictionary<string, object?>();
        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            if (OscAvatarUtility.IsCommonParameter(param.Name))
            {
                continue;
            }

            var outputInterface = param.Output;
            Debug.Assert(outputInterface != null);
            uniqueParamsBuilder.Add(outputInterface.AddressBlob, param);
            uniqueParamValues.Add(param.Name, null);
        }

        _addressToUniqueParam = uniqueParamsBuilder.ToImmutable();
        _paramNameToUniqueParamValues = uniqueParamValues;
        OscUtility.Server.AddMonitorCallback(GetValueCallback);
    }
    public OscAvatarParametorContainer(IEnumerable<OscAvatarParameter> parameters)
        : this(parameters.ToImmutableArray())
    {
    }

    public ImmutableArray<OscAvatarParameter> Parameters { get; }

    private readonly ImmutableDictionary<BlobString, OscAvatarParameter> _addressToUniqueParam;
    private readonly Dictionary<string, object?> _paramNameToUniqueParamValues;

    public ImmutableArray<OscAvatarParameter> UniqueParameters 
        => _addressToUniqueParam.Values.ToImmutableArray();
    public IEnumerable<object?> UniqueParameterValues
        => _paramNameToUniqueParamValues.Values;
    
    public IEnumerable<object?> ParameterValues
        => _paramNameToUniqueParamValues.Values.Concat(OscAvatarUtility.GetCommonParameterValues());

    public IEnumerable<string> Names => Parameters.Select(param => param.Name);

    public object this[string name]
    {
        get => GetAs<object>(name)!;
        set => SetAs<object>(name, value);
    }

    public IEnumerable<string> Keys => Names;

    public IEnumerable<object> Values => Names.Select(a => GetAs<object>(a)!);

    public int Count => Parameters.Length;

    public bool ContainsKey(string key) => Parameters.Any(param => param.Name == key);

    public T? GetAs<T>(string name) where T : notnull
    {
        if (OscAvatarUtility.IsCommonParameter(name))
        {
            return (T?)OscAvatarUtility.GetCommonParameterValue(name) ?? default;
        }
        if (_paramNameToUniqueParamValues.TryGetValue(name, out var value))
        {
            return (T?)value ?? default;
        }
        throw new ArgumentOutOfRangeException(nameof(name));
    }

    private void GetValueCallback(BlobString address, OscMessageValues values)
    {
        if (!_addressToUniqueParam.ContainsKey(address))
        {
            return;
        }

        var param = _addressToUniqueParam[address];
        var paramName = param.Name;
        var paramToValue = _paramNameToUniqueParamValues;

        values.ForEachElement((index, tag) =>
        {
            var oldValue = paramToValue[paramName];
            var newValue = (object?)(tag switch
            {
                TypeTag.Int32 => values.ReadIntElementUnchecked(index),
                TypeTag.Float32 => values.ReadFloatElementUnchecked(index),
                TypeTag.True => true,
                TypeTag.False => false,
                _ => null,
            });
            paramToValue[paramName] = newValue;

            var eventArgs = new ValueChangedEventArgs(oldValue, newValue);
            OnParameterChanged(param, eventArgs);
        });
    }

    public event OscAvatarParameterChangedEventHandler? ParameterChanged;

    protected internal void OnParameterChanged(OscAvatarParameter param, ValueChangedEventArgs e)
    {
        ParameterChanged?.Invoke(param, e);
    }

    public void SetAs<T>(string name, T value)
    {
        var inputInterface = GetParameter(name).Input;
        if (inputInterface == null)
        {
            throw new InvalidOperationException($"{name} dosen't has a input interface.");
        }

        var client = OscUtility.Client;

        switch (value)
        {
            case int intValue:
                client.Send(inputInterface.Address, intValue);
                break;
            case float floatValue:
                client.Send(inputInterface.Address, floatValue);
                break;
            case bool booleanValue:
                client.Send(inputInterface.Address, booleanValue);
                break;
            default:
                throw new NotSupportedException($"The type {value?.GetType()} is still not supported.");
        }
    }

    public OscAvatarParameter GetParameter(string name) => Parameters.First(p => p.Name == name);

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public bool TryGetValue(string key, [NotNullWhen(true)] out object? value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    {
        var param = Parameters.FirstOrDefault();
        if (param == null)
        {
            value = default;
            return false;
        }

        value = GetAs<object>(param.Name);
        return value != null;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        => Parameters
            .Select(param => new KeyValuePair<string, object>(param.Name, GetAs<object>(param.Name)))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
