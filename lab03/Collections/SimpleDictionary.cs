using System.Collections;
using System.Collections.Generic;

namespace Lab03.Collections;

public class SimpleDictionary<TKey, TValue> :
    IDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private struct Entry
    {
        public int HashCode;
        public int Next;
        public TKey Key;
        public TValue Value;
    }

    private const int DefaultCapacity = 4;

    private int[] _buckets = Array.Empty<int>();
    private Entry[] _entries = Array.Empty<Entry>();
    private int _count;
    private int _freeList = -1;
    private int _freeCount;
    private int _version;
    private readonly IEqualityComparer<TKey> _comparer;

    public SimpleDictionary()
        : this(0, EqualityComparer<TKey>.Default)
    {
    }

    public SimpleDictionary(int capacity)
        : this(capacity, EqualityComparer<TKey>.Default)
    {
    }

    public SimpleDictionary(IEqualityComparer<TKey> comparer)
        : this(0, comparer)
    {
    }

    public SimpleDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _comparer = comparer ?? EqualityComparer<TKey>.Default;

        if (capacity > 0)
        {
            Initialize(capacity);
        }
    }

    public int Count => _count - _freeCount;

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys => new KeyCollection(this);

    public ICollection<TValue> Values => new ValueCollection(this);

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    public TValue this[TKey key]
    {
        get
        {
            var index = FindEntry(key);
            if (index >= 0)
            {
                return _entries[index].Value;
            }

            throw new KeyNotFoundException($"ключ нету \"{key}\"");
        }
        set => Insert(key, value, overwriteExisting: true);
    }

    public void Add(TKey key, TValue value) => Insert(key, value, overwriteExisting: false);

    public bool TryGetValue(TKey key, out TValue value)
    {
        var index = FindEntry(key);
        if (index >= 0)
        {
            value = _entries[index].Value;
            return true;
        }

        value = default!;
        return false;
    }

    public bool ContainsKey(TKey key) => FindEntry(key) >= 0;

    public bool Remove(TKey key)
    {
        if (_buckets.Length == 0)
        {
            return false;
        }

        var hashCode = GetHashCode(key);
        var bucket = hashCode % _buckets.Length;
        var last = -1;
        var index = _buckets[bucket] - 1;

        while (index >= 0)
        {
            ref var entry = ref _entries[index];
            if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key))
            {
                if (last < 0)
                {
                    _buckets[bucket] = entry.Next + 1;
                }
                else
                {
                    _entries[last].Next = entry.Next;
                }

                entry.Next = _freeList;
                entry.Key = default!;
                entry.Value = default!;
                entry.HashCode = -1;
                _freeList = index;
                _freeCount++;
                _version++;
                return true;
            }

            last = index;
            index = entry.Next;
        }

        return false;
    }

    public void Clear()
    {
        if (_count == 0)
        {
            return;
        }

        Array.Clear(_buckets, 0, _buckets.Length);
        Array.Clear(_entries, 0, _count);
        _count = 0;
        _freeList = -1;
        _freeCount = 0;
        _version++;
    }

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        var index = FindEntry(item.Key);
        if (index < 0)
        {
            return false;
        }

        return EqualityComparer<TValue>.Default.Equals(_entries[index].Value, item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (arrayIndex < 0 || arrayIndex > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("недостаточно места в массиве", nameof(array));
        }

        var count = _count;
        var entries = _entries;
        for (var i = 0; i < count; i++)
        {
            if (entries[i].HashCode >= 0)
            {
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(entries[i].Key, entries[i].Value);
            }
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var index = FindEntry(item.Key);
        if (index < 0)
        {
            return false;
        }

        if (!EqualityComparer<TValue>.Default.Equals(_entries[index].Value, item.Value))
        {
            return false;
        }

        return Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Insert(TKey key, TValue value, bool overwriteExisting)
    {
        if (_buckets.Length == 0)
        {
            Initialize(DefaultCapacity);
        }

        var hashCode = GetHashCode(key);
        var bucket = hashCode % _buckets.Length;
        var index = _buckets[bucket] - 1;

        while (index >= 0)
        {
            ref var entry = ref _entries[index];
            if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key))
            {
                if (!overwriteExisting)
                {
                    throw new ArgumentException($"ключа есть уеэ \"{key}\"", nameof(key));
                }

                entry.Value = value;
                _version++;
                return;
            }

            index = entry.Next;
        }

        int entryIndex;
        if (_freeCount > 0)
        {
            entryIndex = _freeList;
            _freeList = _entries[entryIndex].Next;
            _freeCount--;
        }
        else
        {
            if (_count == _entries.Length)
            {
                Resize();
                bucket = hashCode % _buckets.Length;
            }

            entryIndex = _count;
            _count++;
        }

        ref var newEntry = ref _entries[entryIndex];
        newEntry.HashCode = hashCode;
        newEntry.Next = _buckets[bucket] - 1;
        newEntry.Key = key;
        newEntry.Value = value;

        _buckets[bucket] = entryIndex + 1;
        _version++;
    }

    private void Initialize(int capacity)
    {
        var size = GetPrime(capacity);
        _buckets = new int[size];
        _entries = new Entry[size];
        _freeList = -1;
    }

    private void Resize()
    {
        var newSize = GetPrime(_count * 2);
        var newBuckets = new int[newSize];
        var newEntries = new Entry[newSize];
        Array.Copy(_entries, 0, newEntries, 0, _count);

        for (var i = 0; i < _count; i++)
        {
            ref var entry = ref newEntries[i];
            if (entry.HashCode < 0)
            {
                continue;
            }

            var bucket = entry.HashCode % newSize;
            entry.Next = newBuckets[bucket] - 1;
            newBuckets[bucket] = i + 1;
        }

        _buckets = newBuckets;
        _entries = newEntries;
    }

    private int FindEntry(TKey key)
    {
        if (_buckets.Length == 0)
        {
            return -1;
        }

        var hashCode = GetHashCode(key);
        var index = _buckets[hashCode % _buckets.Length] - 1;

        while (index >= 0)
        {
            ref var entry = ref _entries[index];
            if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key))
            {
                return index;
            }

            index = entry.Next;
        }

        return -1;
    }

    private int GetHashCode(TKey key) => (_comparer.GetHashCode(key) & 0x7FFFFFFF);

    private static int GetPrime(int min)
    {
        // набор простых чисел для resize
        int[] primes =
        {
            3, 7, 17, 37, 67, 131, 257, 521, 1031, 2053,
            4099, 8209, 16411, 32771, 65537, 131101, 262147, 524309, 1048583
        };

        foreach (var prime in primes)
        {
            if (prime >= min)
            {
                return prime;
            }
        }

        // Если вышли за пределы таблицы — последний элемент * 2
        return primes[^1] * 2 + 1;
    }

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly SimpleDictionary<TKey, TValue> _dictionary;
        private readonly int _version;
        private int _index;
        private KeyValuePair<TKey, TValue> _current;

        internal Enumerator(SimpleDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _version = dictionary._version;
            _index = 0;
            _current = default;
        }

        public KeyValuePair<TKey, TValue> Current => _current;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_version != _dictionary._version)
            {
                throw new InvalidOperationException("коллекция была изменена во время enumeration обхода");
            }

            while ((uint)_index < (uint)_dictionary._count)
            {
                ref var entry = ref _dictionary._entries[_index++];
                if (entry.HashCode >= 0)
                {
                    _current = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
                    return true;
                }
            }

            _index = _dictionary._count + 1;
            _current = default;
            return false;
        }

        public void Reset()
        {
            if (_version != _dictionary._version)
            {
                throw new InvalidOperationException("коллекция была изменена во время enumeration обхода");
            }

            _index = 0;
            _current = default;
        }

        public void Dispose()
        {
        }
    }

    public sealed class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>
    {
        private readonly SimpleDictionary<TKey, TValue> _dictionary;

        internal KeyCollection(SimpleDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => true;

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("недостаточно места в массиве", nameof(array));
            }

            var count = _dictionary._count;
            var entries = _dictionary._entries;
            for (var i = 0; i < count; i++)
            {
                if (entries[i].HashCode >= 0)
                {
                    array[arrayIndex++] = entries[i].Key;
                }
            }
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            foreach (var pair in _dictionary)
            {
                yield return pair.Key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TKey item) => throw new NotSupportedException("коллекция ток для чтения");
        public bool Remove(TKey item) => throw new NotSupportedException("коллекция ток для чтения");
        public void Clear() => throw new NotSupportedException("коллекция ток для чтения");
        public bool Contains(TKey item) => _dictionary.ContainsKey(item);
    }

    public sealed class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>
    {
        private readonly SimpleDictionary<TKey, TValue> _dictionary;

        internal ValueCollection(SimpleDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => true;

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("недостаточно места в массиве", nameof(array));
            }

            var count = _dictionary._count;
            var entries = _dictionary._entries;
            for (var i = 0; i < count; i++)
            {
                if (entries[i].HashCode >= 0)
                {
                    array[arrayIndex++] = entries[i].Value;
                }
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var pair in _dictionary)
            {
                yield return pair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TValue item) => throw new NotSupportedException("коллекция ток для чтения");
        public bool Remove(TValue item) => throw new NotSupportedException("коллекция ток для чтения");
        public void Clear() => throw new NotSupportedException("коллекция ток для чтения");
        public bool Contains(TValue item) => _dictionary.ContainsValue(item);

    }

    private bool ContainsValue(TValue value)
    {
        var comparer = EqualityComparer<TValue>.Default;
        var count = _count;
        var entries = _entries;
        for (var i = 0; i < count; i++)
        {
            if (entries[i].HashCode >= 0 &&
                comparer.Equals(entries[i].Value, value))
            {
                return true;
            }
        }

        return false;
    }
}