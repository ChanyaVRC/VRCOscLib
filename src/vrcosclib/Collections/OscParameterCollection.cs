using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ParamChangedHandler = BuildSoft.VRChat.Osc.OscParameterChangedEventHandler<BuildSoft.VRChat.Osc.IReadOnlyOscParameterCollection>;

namespace BuildSoft.VRChat.Osc;

public class OscParameterCollection : IDictionary<string, object?>, IReadOnlyOscParameterCollection
{
    private readonly Dictionary<string, object?> _items = new();

    private Dictionary<string, List<ParamChangedHandler>>? _handlersPerAddress;
    private Dictionary<string, List<ParamChangedHandler>> HandlersPerAddress => _handlersPerAddress ??= new();

    public object? this[string address]
    {
        get => _items[address];
        set
        {
            bool containsValue = _items.TryGetValue(address, out var oldValue);
            if (!containsValue || !OscUtility.AreEqual(oldValue, value))
            {
                _items[address] = value;
                var reason = containsValue ? ValueChangedReason.Substituted : ValueChangedReason.Added;
                OnValueChanged(new ParameterChangedEventArgs(oldValue, value, address, reason));
            }
        }
    }

    public ICollection<string> Keys => _items.Keys;

    public ICollection<object?> Values => _items.Values;

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public void Add(string address, object? value)
    {
        _items.Add(address, value);
        OnValueChanged(new ParameterChangedEventArgs(null, value, address, ValueChangedReason.Added));
    }

    public void Add(KeyValuePair<string, object?> item)
       => Add(item.Key, item.Value);

    public void Clear()
    {
        var copiedItems = _items.ToArray();
        _items.Clear();
        for (int i = 0; i < copiedItems.Length; i++)
        {
            OnValueChanged(new ParameterChangedEventArgs(copiedItems[i].Value, null, copiedItems[i].Key, ValueChangedReason.Removed));
        }
    }

    public bool Contains(KeyValuePair<string, object?> item)
        => _items.TryGetValue(item.Key, out var value) && item.Value == value;

    public bool ContainsKey(string key) => _items.ContainsKey(key);

    public bool Remove(string key)
    {
        bool removed = _items.Remove(key, out var value);
        if (removed)
        {
            OnValueChanged(new ParameterChangedEventArgs(value, null, key, ValueChangedReason.Removed));
        }
        return removed;
    }

    public bool TryGetValue(string key, out object? value) => _items.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _items.GetEnumerator();

    #region Interface methods implemented explicitly
    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => Keys;
    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => Values;

    void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, object?>>)_items).CopyTo(array, arrayIndex);
    }
    bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
    {
        return ((ICollection<KeyValuePair<string, object?>>)_items).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion

    #region Event(s)
    public event ParamChangedHandler? ValueChanged;

    protected void OnValueChanged(ParameterChangedEventArgs args)
    {
        ValueChanged?.Invoke(this, args);
        OnValueChangedByAddress(args);
    }

    protected void OnValueChangedByAddress(ParameterChangedEventArgs args)
    {
        if (!HandlersPerAddress.TryGetValue(args.Address, out var list) || list.Count <= 0)
        {
            return;
        }
        var handlers = list.ToArray();
        for (int i = 0; i < handlers.Length; i++)
        {
            handlers[i].Invoke(this, args);
        }
    }

    #region Event registration methods
    public void AddValueChangedEventByAddress(string address, ParamChangedHandler handler)
    {
        var dict = HandlersPerAddress;
        if (dict.TryGetValue(address, out var list))
        {
            list.Add(handler);
            return;
        }
        dict.Add(address, new() { handler });
    }

    public bool RemoveValueChangedEventByAddress(string address, ParamChangedHandler handler)
    {
        var dict = HandlersPerAddress;
        if (!dict.TryGetValue(address, out var list))
        {
            return false;
        }
        return list.Remove(handler);
    }
    #endregion
    #endregion
}
