using System.Collections;
using System.Collections.Immutable;
using BuildSoft.VRChat.Osc.Avatar;

namespace ExpressionAvatarChanger;

public class AvatarCollection : IList<Avatar>
{
    private readonly List<Avatar> _items = [];

    public AvatarCollection()
    {
        
    }

    public AvatarCollection(IEnumerable<Avatar> collection) : this()
    {
        AddRange(collection);
    }

    public AvatarCollection(IEnumerable<OscAvatarConfig> collection) : this()
    {
        AddRange(collection);
    }

    public Avatar this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public int Count => _items.Count;

    bool ICollection<Avatar>.IsReadOnly => false;

    public void Add(Avatar item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        _items.Add(item);
    }

    public void Add(OscAvatarConfig item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        var parameters = item.Parameters.Items
            .Where(v => v.Input != null)
            .Select(v => (v.Name, v.Input!.OscType))
            .ToImmutableArray();
        _items.Add(new(item.Id, item.Name, parameters));
    }

    public void AddRange(IEnumerable<Avatar> collection)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));

        if (collection is ICollection<Avatar> c)
        {
            if (_items.Capacity < _items.Count + c.Count)
            {
                _items.Capacity = _items.Count + c.Count;
            }
        }
        foreach (var item in collection)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(collection));
            Add(item);
        }
    }

    public void AddRange(IEnumerable<OscAvatarConfig> collection)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));

        if (collection is ICollection<OscAvatarParameter> c)
        {
            if (_items.Capacity < _items.Count + c.Count)
            {
                _items.Capacity = _items.Count + c.Count;
            }
        }
        foreach (var item in collection)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(collection));
            Add(item);
        }
    }

    public void Clear() => _items.Clear();

    public bool Contains(Avatar item) => _items.Contains(item);

    public void CopyTo(Avatar[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    public IEnumerator<Avatar> GetEnumerator() => _items.GetEnumerator();

    public int IndexOf(Avatar item) => _items.IndexOf(item);

    public void Insert(int index, Avatar item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Insert(index, item);
    }

    public bool Remove(Avatar item) => _items.Remove(item);

    public void RemoveAt(int index) => _items.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
