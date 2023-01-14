using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BuildSoft.VRChat.Osc.Delegate;
using ParamChangedHandler = BuildSoft.VRChat.Osc.OscParameterChangedEventHandler<BuildSoft.VRChat.Osc.IReadOnlyOscParameterCollection>;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// A collection of OSC parameters that can be accessed using a string address.
/// </summary>
internal class OscParameterCollection : IDictionary<string, object?>, IReadOnlyOscParameterCollection
{
    private readonly Dictionary<string, object?> _items = new();

    private Dictionary<string, List<ParamChangedHandler>>? _handlersPerAddress;

    /// <inheritdoc/>
    public object? this[string address]
    {
        get => _items[address];
        set => SetValue(address, value, ValueSource.Application);
    }

    internal void SetValue(string address, object? value, ValueSource valueSource)
    {
        bool containsValue = _items.TryGetValue(address, out var oldValue);
        if (!containsValue || !OscUtility.AreEqual(oldValue, value))
        {
            _items[address] = value;
            var reason = containsValue ? ValueChangedReason.Substituted : ValueChangedReason.Added;
            OnValueChanged(new ParameterChangedEventArgs(oldValue, value, address, reason, valueSource));
        }
    }

    /// <inheritdoc/>
    public ICollection<string> Keys => _items.Keys;

    /// <inheritdoc/>
    public ICollection<object?> Values => _items.Values;

    /// <inheritdoc/>
    public int Count => _items.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(string address, object? value)
    {
        _items.Add(address, value);
        OnValueChanged(new ParameterChangedEventArgs(null, value, address, ValueChangedReason.Added, ValueSource.Application));
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<string, object?> item)
       => Add(item.Key, item.Value);

    /// <inheritdoc/>
    public void Clear()
    {
        var copiedItems = _items.ToArray();
        _items.Clear();
        for (int i = 0; i < copiedItems.Length; i++)
        {
            OnValueChanged(new ParameterChangedEventArgs(copiedItems[i].Value, null, copiedItems[i].Key, ValueChangedReason.Removed, ValueSource.Application));
        }
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<string, object?> item)
        => _items.TryGetValue(item.Key, out var value) && item.Value == value;

    /// <inheritdoc/>
    public bool ContainsKey(string key) => _items.ContainsKey(key);

    /// <inheritdoc/>
    public bool Remove(string key)
    {
        var item = _items;
        if (item.TryGetValue(key, out var value))
        {
            bool removed = item.Remove(key);
            if (removed)
            {
                OnValueChanged(new ParameterChangedEventArgs(value, null, key, ValueChangedReason.Removed, ValueSource.Application));
            }
            return removed;
        }
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetValue(string key, out object? value) => _items.TryGetValue(key, out value);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _items.GetEnumerator();

    #region Interface methods implemented explicitly
    /// <inheritdoc/>
    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => Keys;
    /// <inheritdoc/>
    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => Values;

    /// <inheritdoc/>
    void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, object?>>)_items).CopyTo(array, arrayIndex);
    }
    /// <inheritdoc/>
    bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
    {
        return ((ICollection<KeyValuePair<string, object?>>)_items).Remove(item);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion

    #region Event(s)

    /// <inheritdoc/>
    public event ParamChangedHandler? ValueChanged;

    /// <summary>
    /// Raises the <see cref="ValueChanged"/> event and invokes the event handlers registered by address.
    /// </summary>
    /// <param name="args">The event data.</param>
    protected void OnValueChanged(ParameterChangedEventArgs args)
    {
        ValueChanged?.DynamicInvokeAllWithoutException(this, args);
        OnValueChangedByAddress(args);
    }

    /// <summary>
    /// Invokes the event handlers registered by address for the <see cref="ValueChanged"/> event.
    /// </summary>
    /// <param name="args">The event data.</param>
    protected void OnValueChangedByAddress(ParameterChangedEventArgs args)
    {
        var handlersPerAddress = _handlersPerAddress;
        if (handlersPerAddress == null)
        {
            return;
        }
        if (!handlersPerAddress.TryGetValue(args.Address, out var list) || list.Count <= 0)
        {
            return;
        }

        var handlers = list.ToArray();
        for (int i = 0; i < handlers.Length; i++)
        {
            try
            {
                handlers[i].Invoke(this, args);
            }
            catch (Exception)
            {
                // eat exception
            }
        }
    }

    #region Event registration methods

    /// <inheritdoc/>
    public void AddValueChangedEventByAddress(string address, ParamChangedHandler handler)
    {
        var dict = _handlersPerAddress;
        if (dict == null)
        {
            dict = new();
            _handlersPerAddress = dict;
        }
        else if (dict.TryGetValue(address, out var list))
        {
            list.Add(handler);
            return;
        }
        dict.Add(address, new() { handler });
    }

    /// <inheritdoc/>
    public bool RemoveValueChangedEventByAddress(string address, ParamChangedHandler handler)
    {
        var dict = _handlersPerAddress;
        if (dict == null)
        {
            return false;
        }
        if (!dict.TryGetValue(address, out var list))
        {
            return false;
        }
        return list.Remove(handler);
    }
    #endregion
    #endregion
}
