using System.Diagnostics.CodeAnalysis;

namespace ForsMachine.Utils;

public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
{
    private Dictionary<TKey, TValue> _dictionary = new();

    private List<TKey> _keys = new();

    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
        _keys.Add(key);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        _dictionary.Clear();
        _keys.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var key in _keys)
        {
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
        }
    }

    public bool IsReadOnly => false;

    public bool Remove(TKey key)
    {
        if (!_dictionary.Remove(key))
        {
            return false;
        }
        _keys.Remove(key);
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public ICollection<TKey> Keys => _keys;

    public ICollection<TValue> Values => _keys.Select(key => _dictionary[key]).ToList();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var key in _keys)
        {
            yield return new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Count => _dictionary.Count;

    public IEnumerable<TKey> Items
    {
        get
        {
            foreach (var key in _keys)
            {
                yield return key;
            }
        }
    }

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            if (!_dictionary.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            _dictionary[key] = value;
        }
    }
}
