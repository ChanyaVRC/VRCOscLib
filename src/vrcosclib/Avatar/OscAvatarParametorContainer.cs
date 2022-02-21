using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BlobHandles;
using BuildSoft.OscCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildSoft.VRChat.Osc;

public class OscAvatarParametorContainer : IReadOnlyDictionary<string, object>
{
    public OscAvatarParametorContainer(IEnumerable<OscAvatarParameter> parameters)
    {
        Parameters = parameters.ToImmutableArray();
        ParameterValues = new object[Parameters.Length];
        OscUtility.Server.AddMonitorCallback(GetValueCallback);
    }

    public ImmutableArray<OscAvatarParameter> Parameters { get; }
    public object?[] ParameterValues { get; }

    public IEnumerable<string> Names => Parameters.Select(param => param.Name);

    public object this[string name]
    {
        get => GetAs<object>(name);
        set => SetAs<object>(name, value);
    }

    public IEnumerable<string> Keys => Names;

    public IEnumerable<object> Values => Names.Select(a => GetAs<object>(a));

    public int Count => Parameters.Length;

    public bool ContainsKey(string key) => Parameters.Any(param => param.Name == key);

    public T GetAs<T>(string name) where T : notnull
    {
        int i;
        var parameters = Parameters;
        for (i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].Name == name)
            {
                break;
            }
        }
        if (i >= parameters.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(name));
        }
        if (parameters[i].Output == null)
        {
            throw new InvalidOperationException($"{name} dosen't has a output interface.");
        }

        return (T)(ParameterValues[i] ?? default(T) ?? new object());
    }

    private void GetValueCallback(BlobString address, OscMessageValues values)
    {
        var parameters = Parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            var outputInterface = parameters[i].Output;
            if (outputInterface == null || address != outputInterface.AddressBlob)
            {
                continue;
            }

            values.ForEachElement((index, tag) =>
            {
                if (index != values.ElementCount - 1)
                {
                    return;
                }

                var oldValue = ParameterValues[i];

                ParameterValues[i] = tag switch
                {
                    TypeTag.Int32 => values.ReadIntElementUnchecked(index),
                    TypeTag.Float32 => values.ReadFloatElementUnchecked(index),
                    TypeTag.True => true,
                    TypeTag.False => false,
                    _ => null,
                };

                var eventArgs = new ValueChangedEventArgs(oldValue, ParameterValues[i]);
                OnParameterChanged?.Invoke(parameters[i], eventArgs);
            });

            break;
        }
    }

    public event OscAvatarParameterChangedEventHandler? OnParameterChanged;

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
        return true;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        => Parameters
            .Select(param => new KeyValuePair<string, object>(param.Name, GetAs<object>(param.Name)))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
