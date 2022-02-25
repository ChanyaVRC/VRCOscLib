using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ParamChangedHandler = BuildSoft.VRChat.Osc.OscParameterChangedEventHandler<BuildSoft.VRChat.Osc.OscParameterCollection>;

namespace BuildSoft.VRChat.Osc;

public class OscParameterCollection : IDictionary<string, object?>
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
            if (!containsValue || oldValue != value)
            {
                _items[address] = value;
                OnValueChanged(new ParameterChangedEventArgs(oldValue, value, address));
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
        OnValueChanged(new ParameterChangedEventArgs(null, value, address));
    }

    public void Add(KeyValuePair<string, object?> item)
       => Add(item.Key, item.Value);

    public void Clear() => _items.Clear();

    public bool Contains(KeyValuePair<string, object?> item)
        => _items.TryGetValue(item.Key, out var value) && item.Value == value;

    public bool ContainsKey(string key) => _items.ContainsKey(key);

    public bool Remove(string key) => _items.Remove(key);

    public bool TryGetValue(string key, out object? value) => _items.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _items.GetEnumerator();

    #region Interface methods implemented explicitly
    void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        throw new NotSupportedException();
    }
    bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
    {
        throw new NotSupportedException();
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
