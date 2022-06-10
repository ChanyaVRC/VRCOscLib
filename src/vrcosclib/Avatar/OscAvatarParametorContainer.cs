using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BlobHandles;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc.Avatar;

public class OscAvatarParametorContainer : IReadOnlyDictionary<string, object?>
{
    #region Static methods(s)
    /// <summary>
    /// Call initializer of dependencies.
    /// </summary>
    internal static void Initialize()
    {
    }

    /// <summary>
    /// Initialize static members.
    /// </summary>
    static OscAvatarParametorContainer()
    {
        OscParameter.Initialize();
    }
    #endregion

    #region Constructor(s)
    public OscAvatarParametorContainer(ImmutableArray<OscAvatarParameter> parameters)
    {
        Items = parameters;

        var allParams = OscParameter.Parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var outputInterface = param.Output;
            Debug.Assert(outputInterface != null);
            allParams.AddValueChangedEventByAddress(outputInterface.Address, GetValueCallback);
        }
    }

    public OscAvatarParametorContainer(IEnumerable<OscAvatarParameter> parameters)
        : this(parameters.ToImmutableArray())
    {
    }
    #endregion

    #region Datas
    public ImmutableArray<OscAvatarParameter> Items { get; }
    public OscAvatarParameter Get(string name) => Items.First(p => p.Name == name);

    public IEnumerable<OscAvatarParameter> UniqueParameters => Items.Where(parm => !OscAvatarUtility.IsCommonParameter(parm.Name));
    public IEnumerable<object?> UniqueParameterValues
    {
        get
        {
            var allParams = OscParameter.Parameters;
            foreach (var item in UniqueParameters)
            {
                var address = item.ReadableAddress;
                allParams.TryGetValue(address, out var value);
                yield return value;
            }
        }
    }

    public IEnumerable<string> Names => Items.Select(param => param.Name);

    public IEnumerable<string> Keys => Names;
    public IEnumerable<object?> Values => Items.Select(v => GetAs<object>(v.Name));

    public int Count => Items.Length;
    #endregion

    #region Value accessor(s)
    public object? this[string name]
    {
        get => GetAs<object>(name);
        set => SetAs<object?>(name, value);
    }

    public T? GetAs<T>(string name) where T : notnull
    {
        var param = Get(name);
        var allParams = OscParameter.Parameters;
        if (allParams.TryGetValue(param.ReadableAddress, out var value))
        {
            return (T?)value ?? default;
        }
        throw new ArgumentOutOfRangeException(nameof(name));
    }

    public void SetAs<T>(string name, T value)
    {
        var inputInterface = Get(name).Input;
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

    public bool ContainsKey(string key) => Items.Any(param => param.Name == key);

    public bool TryGetValue(string key, [NotNullWhen(true)] out object? value)
    {
        var param = Items.FirstOrDefault();
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
    private void GetValueCallback(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
    {
        var name = e.Address[OscConst.AvatarParameterAddressSpace.Length..];
        OnParameterChanged(Get(name), e);
    }

    public event OscAvatarParameterChangedEventHandler? ParameterChanged;

    protected internal void OnParameterChanged(OscAvatarParameter param, ValueChangedEventArgs e)
    {
        ParameterChanged?.Invoke(param, e);
    }
    #endregion

    #region GetEnumerator method(s)
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        => Items
            .Select(param => new KeyValuePair<string, object?>(param.Name, GetAs<object>(param.Name)))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
}
