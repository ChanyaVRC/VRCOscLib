using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BlobHandles;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public class OscAvatarParametorContainer : IReadOnlyDictionary<string, object?>
{
    #region Static methods(s)
    internal static void Initialize()
    {

    }
    #endregion

    #region Constructor(s)
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
    #endregion

    #region Datas
    private readonly ImmutableDictionary<BlobString, OscAvatarParameter> _addressToUniqueParam;
    private readonly Dictionary<string, object?> _paramNameToUniqueParamValues;

    public ImmutableArray<OscAvatarParameter> Parameters { get; }
    public OscAvatarParameter GetParameter(string name) => Parameters.First(p => p.Name == name);

    public IEnumerable<OscAvatarParameter> UniqueParameters => _addressToUniqueParam.Values;
    public IEnumerable<object?> UniqueParameterValues => _paramNameToUniqueParamValues.Values;

    public IEnumerable<string> Names => Parameters.Select(param => param.Name);

    public IEnumerable<string> Keys => Names;
    public IEnumerable<object?> Values => Parameters.Select(v => GetAs<object>(v.Name));

    public int Count => Parameters.Length;
    #endregion

    #region Value accessor(s)
    public object this[string name]
    {
        get => GetAs<object>(name)!;
        set => SetAs<object>(name, value);
    }

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

    public bool ContainsKey(string key) => Parameters.Any(param => param.Name == key);

    public bool TryGetValue(string key, [NotNullWhen(true)] out object? value)
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
    #endregion

    #region Events
    private void GetValueCallback(BlobString address, OscMessageValues values)
    {
        if (!_addressToUniqueParam.ContainsKey(address))
        {
            return;
        }

        var param = _addressToUniqueParam[address];
        var paramName = param.Name;
        var paramToValue = _paramNameToUniqueParamValues;

        for (int i = 0; i < values.ElementCount; i++)
        {
            var oldValue = paramToValue[paramName];
            var newValue = values.ReadValue(i);
            paramToValue[paramName] = newValue;

            var eventArgs = new ValueChangedEventArgs(oldValue, newValue);
            OnParameterChanged(param, eventArgs);
        }
    }

    public event OscAvatarParameterChangedEventHandler? ParameterChanged;

    protected internal void OnParameterChanged(OscAvatarParameter param, ValueChangedEventArgs e)
    {
        ParameterChanged?.Invoke(param, e);
    }
    #endregion

    #region GetEnumerator method(s)
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        => Parameters
            .Select(param => new KeyValuePair<string, object?>(param.Name, GetAs<object>(param.Name)))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
}
