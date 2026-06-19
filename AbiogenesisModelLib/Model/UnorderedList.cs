using System.Collections;
using AbiogenesisModel.Lib.Guard;

namespace AbiogenesisModel.Lib.Model;

/// <summary>
/// Unordered collection with O(1) average Add / Remove / Contains
/// and O(1) index-based access.
/// Element order is not preserved after removal.
/// </summary>
internal class UnorderedList<T> : ICollection<T>, IReadOnlyList<T>
    where T : class
{
    private readonly List<T> _items;
    private readonly Dictionary<T, int> _indexMap;

    internal UnorderedList(int capacity = 0)
    {
        Ensure.That(capacity).IsGreaterOrEqual(0);

        _items = new List<T>(capacity);
        _indexMap = new Dictionary<T, int>(capacity, ReferenceEqualityComparer.Instance);
    }

    internal UnorderedList(IReadOnlyList<T> source)
    {
        Ensure.That(source).IsNotNull().IsNullFree().IsDuplicateFree();

        _items = new List<T>(source);
        _indexMap = new Dictionary<T, int>(source.Count, ReferenceEqualityComparer.Instance);
        for (var i = 0; i < source.Count; i++)
        {
            _indexMap[source[i]] = i;
        }
    }

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public int Capacity
    {
        get => _items.Capacity;
        set
        {
            Ensure.That(value).IsGreaterOrEqual(_items.Count);

            _items.Capacity = value;
            _indexMap.EnsureCapacity(value);
        }
    }

    public T this[int index] => _items[index];

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Adds an item. Throws if the item is already present.
    /// </summary>
    public void Add(T item)
    {
        Ensure.That(item).IsNotNull().IsNotKeyOf(_indexMap);

        var index = _items.Count;
        _items.Add(item);
        _indexMap.Add(item, index);
    }

    public void Clear()
    {
        _items.Clear();
        _indexMap.Clear();
    }

    public bool Contains(T item)
    {
        return _indexMap.ContainsKey(item);
    }

    public bool TryGetIndex(T item, out int index)
    {
        return _indexMap.TryGetValue(item, out index);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Ensure.That(array).IsNotNull();
        Ensure.That(arrayIndex).IsInRangeOfIndexes(array);
        Ensure.That(array.Length - arrayIndex).IsGreaterOrEqual(_items.Count);

        _items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the given item in O(1) average time.
    /// Order is not preserved.
    /// </summary>
    public bool Remove(T item)
    {
        Ensure.That(item).IsKeyOf(_indexMap);

        RemoveAtInternal(_indexMap[item]);
        return true;
    }

    /// <summary>
    /// Removes the item at the given index in O(1).
    /// Order is not preserved.
    /// </summary>
    public void RemoveAt(int index)
    {
        Ensure.That(index).IsInRangeOfIndexes(_items);

        RemoveAtInternal(index);
    }

    public void EnsureCapacity(int capacity)
    {
        Ensure.That(capacity).IsGreaterOrEqual(0);

        if (_items.Capacity < capacity)
        {
            _items.Capacity = capacity;
        }

        _indexMap.EnsureCapacity(capacity);
    }

    public void TrimExcess()
    {
        _items.TrimExcess();
    }

    public T GetRandom(Random random)
    {
        Ensure.That(random).IsNotNull();
        Ensure.That(_items).IsNotEmpty();

        var index = random.Next(_items.Count);
        return _items[index];
    }

    public bool TryGetRandom(Random random, out T? item)
    {
        Ensure.That(random).IsNotNull();

        if (_items.Count == 0)
        {
            item = null;
            return false;
        }

        var index = random.Next(_items.Count);
        item = _items[index];
        return true;
    }
    private void RemoveAtInternal(int index)
    {
        var lastIndex = _items.Count - 1;
        var removedItem = _items[index];
        var lastItem = _items[lastIndex];

        if (index != lastIndex)
        {
            _items[index] = lastItem;
            _indexMap[lastItem] = index;
        }

        _items.RemoveAt(lastIndex);
        _indexMap.Remove(removedItem);
    }
}