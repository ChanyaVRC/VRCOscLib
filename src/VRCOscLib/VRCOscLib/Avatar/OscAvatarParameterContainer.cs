using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BlobHandles;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Delegate;

namespace BuildSoft.VRChat.Osc.Avatar;

/// <summary>
/// This class represents a container for avatar parameters in VRChat.
/// </summary>
public class OscAvatarParameterContainer : IReadOnlyDictionary<string, object?>
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
    static OscAvatarParameterContainer()
    {
        Initialize();
    }
    #endregion

    #region Constructor(s)

    /// <summary>
    /// Creates a new instance of <see cref="OscAvatarParameterContainer"/> with the specified avatar parameters.
    /// </summary>
    /// <param name="parameters">The avatar parameters to be contained in this instance.</param>
    public OscAvatarParameterContainer(ImmutableArray<OscAvatarParameter> parameters)
    {
        Items = parameters;

        var allParams = OscParameter.Parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var outputInterface = param.Output;
            Debug.Assert(outputInterface != null);
            allParams.AddValueChangedEventByAddress(outputInterface!.Address, GetValueCallback);
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="OscAvatarParameterContainer"/> with the specified avatar parameters.
    /// </summary>
    /// <param name="parameters">The avatar parameters to be contained in this instance.</param>
    public OscAvatarParameterContainer(IEnumerable<OscAvatarParameter> parameters)
        : this(parameters.ToImmutableArray())
    {
    }

    #endregion

    #region Datas
    /// <summary>
    /// Gets the avatar parameters contained in this instance.
    /// </summary>
    public ImmutableArray<OscAvatarParameter> Items { get; }

    /// <summary>
    /// Gets the avatar parameter with the specified name.
    /// </summary>
    /// <param name="name">The name of the avatar parameter to retrieve.</param>
    /// <returns>The avatar parameter with the specified name.</returns>
    public OscAvatarParameter Get(string name) => Items.First(p => p.Name == name);

    /// <summary>
    /// Attempts to retrieve the avatar parameter with the specified name.
    /// </summary>
    /// <param name="name">The name of the avatar parameter to retrieve.</param>
    /// <returns>The avatar parameter with the specified name, if it exists; otherwise, null.</returns>
    internal OscAvatarParameter? TryGet(string name) => Items.FirstOrDefault(p => p.Name == name);

    /// <summary>
    /// Gets a collection of the unique avatar parameters contained in this instance.
    /// </summary>
    public IEnumerable<OscAvatarParameter> UniqueParameters => Items.Where(param => !OscAvatarUtility.IsCommonParameter(param.Name));

    /// <summary>
    /// Gets a collection of the values of the unique avatar parameters contained in this instance.
    /// </summary>
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

    /// <summary>
    /// Gets a collection of the names of the avatar parameters contained in this instance.
    /// </summary>
    public IEnumerable<string> Names => Items.Select(param => param.Name);

    /// <summary>
    /// Gets a collection of the keys (i.e., names) of the avatar parameters contained in this instance.
    /// </summary>
    public IEnumerable<string> Keys => Names;

    /// <summary>
    /// Gets a collection of the values of the avatar parameters contained in this instance.
    /// </summary>
    public IEnumerable<object?> Values => Items.Select(v => GetAs<object>(v.Name));

    /// <summary>
    /// Gets the number of avatar parameters contained in this instance.
    /// </summary>
    public int Count => Items.Length;

    private ImmutableArray<OscPhysBone> _physBones;

    /// <summary>
    /// Gets a <see cref="OscPhysBone"/> contained in this instance.
    /// </summary>
    public IReadOnlyList<OscPhysBone> PhysBones
    {
        get
        {
            if (_physBones.IsDefault)
            {
                _physBones = CreatePhysBones();
            }
            return _physBones;
        }
    }

    private ImmutableArray<OscPhysBone> CreatePhysBones()
    {
        (string Suffix, OscType Type)[] paramInfos =
        [
            ("_" + nameof(OscPhysBone.IsGrabbed),   OscType.Bool),
            ("_" + nameof(OscPhysBone.IsPosed),     OscType.Bool),
            ("_" + nameof(OscPhysBone.Angle),       OscType.Float),
            ("_" + nameof(OscPhysBone.Stretch),     OscType.Float),
            ("_" + nameof(OscPhysBone.Squish),      OscType.Float),
        ];

        Dictionary<string, int> dictionary = [];
        var items = Items;
        var builder = ImmutableArray.CreateBuilder<OscPhysBone>();

        for (int i = 0; i < items.Length; i++)
        {
            var parameter = items[i];
            var paramName = parameter.Name;
            var output = parameter.Output;

            if (output == null)
            {
                continue;
            }

            for (int j = 0; j < paramInfos.Length; j++)
            {
                var info = paramInfos[j];
                if (!paramName.EndsWith(info.Suffix) || output.OscType != info.Type)
                {
                    continue;
                }

                string baseName = paramName.Substring(0, paramName.Length - info.Suffix.Length);
                int count = dictionary.ContainsKey(baseName) ? dictionary[baseName] + 1 : 1;
                dictionary[baseName] = count;

                if (count == paramInfos.Length)
                {
                    builder.Add(new OscPhysBone(this, baseName, false));
                }

                break;
            }
        }

        return builder.ToImmutable();
    }

    #endregion

    #region Value accessor(s)

    /// <summary>
    /// Gets or sets the value of the avatar parameter with the specified name.
    /// </summary>
    /// <param name="name">The name of the avatar parameter whose value to get or set.</param>
    public object? this[string name]
    {
        get => GetAs<object>(name);
        set => SetAs<object?>(name, value);
    }

    /// <summary>
    /// Gets the value of the avatar parameter with the specified name as the specified type.
    /// </summary>
    /// <typeparam name="T">The type to retrieve the avatar parameter value as.</typeparam>
    /// <param name="name">The name of the avatar parameter whose value to retrieve.</param>
    /// <returns>The value of the avatar parameter as the specified type.</returns>
    public T? GetAs<T>(string name) where T : notnull
    {
        var param = Get(name);
        var allParams = OscParameter.Parameters;
        if (allParams.TryGetValue(param.ReadableAddress, out var value))
        {
            return (T?)value;
        }
        return default;
    }

    /// <summary>
    /// Sets the value of the avatar parameter with the specified name to the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value to set the avatar parameter to.</typeparam>
    /// <param name="name">The name of the avatar parameter whose value to set.</param>
    /// <param name="value">The value to set the avatar parameter to.</param>
    public void SetAs<T>(string name, T value)
    {
        var inputInterface = Get(name).Input;
        if (inputInterface == null)
        {
            throw new InvalidOperationException($"{name} dosen't has a input interface.");
        }

        switch (value)
        {
            case int intValue:
                OscParameter.SendValue(inputInterface.Address, intValue);
                break;
            case float floatValue:
                OscParameter.SendValue(inputInterface.Address, floatValue);
                break;
            case bool booleanValue:
                OscParameter.SendValue(inputInterface.Address, booleanValue);
                break;
            default:
                throw new NotSupportedException($"The type {value?.GetType()} is still not supported.");
        }
    }

    /// <summary>
    /// Determines whether this instance contains an avatar parameter with the specified name.
    /// </summary>
    /// <param name="key">The name of the avatar parameter to locate in this instance.</param>
    /// <returns><see langword="true"/> if this instance contains an avatar parameter with the specified name; otherwise, <see langword="false"/>.</returns>
    public bool ContainsKey(string key) => Items.Any(param => param.Name == key);

    /// <summary>
    /// Tries to get the value of the avatar parameter with the specified name.
    /// </summary>
    /// <param name="key">The name of the avatar parameter whose value to get.</param>
    /// <param name="value">When this method returns, contains the value of the avatar parameter with the specified name, if found;
    /// otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
    /// <returns><see langword="true"/> if the avatar parameter with the specified name is found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue(string key,
#if NETSTANDARD2_1_OR_GREATER
    [NotNullWhen(true)] 
#endif
    out object? value)
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
    /// <summary>
    /// Callback method that is called when the value of an avatar parameter changes.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event data.</param>
    private void GetValueCallback(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
    {
        var name = e.Address.Substring(OscConst.AvatarParameterAddressSpace.Length);
        OscAvatarParameter? param = TryGet(name);
        if (param == null)
        {
            return;
        }

        OnParameterChanged(param, e);
    }

    /// <summary>
    /// Occurs when the value of an avatar parameter contained in this instance changes.
    /// </summary>
    public event OscAvatarParameterChangedEventHandler? ParameterChanged;

    /// <summary>
    /// Raises the <see cref="ParameterChanged"/> event.
    /// </summary>
    /// <param name="param">The avatar parameter whose value changed.</param>
    /// <param name="e">The event data.</param>
    protected internal void OnParameterChanged(OscAvatarParameter param, ValueChangedEventArgs e)
    {
        ParameterChanged?.DynamicInvokeAllWithoutException(param, e);
    }
    #endregion

    #region GetEnumerator method(s)
    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        => Items
            .Select(param => new KeyValuePair<string, object?>(param.Name, GetAs<object>(param.Name)))
            .GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
}
