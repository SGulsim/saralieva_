using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Lab03.Collections;

public class SimpleList<T> : IList<T>, IList, IReadOnlyList<T>
{
    private const int DefaultCapacity = 4;

    private T[] _items;
    private int _count;
    private int _version;

    public SimpleList()
    {
        _items = Array.Empty<T>();
    }

    public SimpleList(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _items = capacity == 0 ? Array.Empty<T>() : new T[capacity];
    }

    public int Count => _count;
    public int Capacity
    {
        get => _items.Length;
        set
        {
            if (value < _count)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "нельзя сделать меньше, у тебя и так мало");
            }

            if (value == _items.Length)
            {
                return;
            }

            if (value == 0)
            {
                _items = Array.Empty<T>();
            }
            else
            {
                var newArray = new T[value];
                if (_count > 0)
                {
                    Array.Copy(_items, newArray, _count);
                }
                _items = newArray;
            }
        }
    }

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public T this[int index]
    {
        get
        {
            EnsureIndexInRange(index);
            return _items[index];
        }
        set
        {
            EnsureIndexInRange(index);
            _items[index] = value;
            _version++;
        }
    }

    object? IList.this[int index]
    {
        get => this[index];
        set
        {
            ValidateAssignable(value, out var typed);
            this[index] = typed;
        }
    }

    public void Add(T item)
    {
        EnsureCapacity(_count + 1);
        _items[_count++] = item;
        _version++;
    }

    int IList.Add(object? value)
    {
        ValidateAssignable(value, out var typed);
        Add(typed);
        return _count - 1;
    }

    public void Clear()
    {
        Array.Clear(_items, 0, _count);
        _count = 0;
        _version++;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    bool IList.Contains(object? value) => IsCompatibleObject(value) && Contains((T)value!);

    public void CopyTo(T[] array, int arrayIndex)
    {
        ValidateCopyTo(array, arrayIndex);
        Array.Copy(_items, 0, array, arrayIndex, _count);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (array.Rank != 1)
        {
            throw new ArgumentException("массив должен быть одномерным", nameof(array));
        }

        if (index < 0 || index > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (array.Length - index < _count)
        {
            throw new ArgumentException("недостаточно места в массиве", nameof(array));
        }

        try
        {
            Array.Copy(_items, 0, array, index, _count);
        }
        catch (ArrayTypeMismatchException)
        {
            throw new ArgumentException("недостаточно места в массиве", nameof(array));
        }
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < _count; i++)
        {
            if (comparer.Equals(_items[i], item))
            {
                return i;
            }
        }

        return -1;
    }

    int IList.IndexOf(object? value) => IsCompatibleObject(value) ? IndexOf((T)value!) : -1;

    public void Insert(int index, T item)
    {
        if (index < 0 || index > _count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        EnsureCapacity(_count + 1);

        if (index < _count)
        {
            Array.Copy(_items, index, _items, index + 1, _count - index);
        }

        _items[index] = item;
        _count++;
        _version++;
    }

    void IList.Insert(int index, object? value)
    {
        ValidateAssignable(value, out var typed);
        Insert(index, typed);
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index < 0)
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    void IList.Remove(object? value)
    {
        if (IsCompatibleObject(value))
        {
            Remove((T)value!);
        }
    }

    public void RemoveAt(int index)
    {
        EnsureIndexInRange(index);
        _count--;
        if (index < _count)
        {
            Array.Copy(_items, index + 1, _items, index, _count - index);
        }

        _items[_count] = default!;
        _version++;
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    private void EnsureCapacity(int min)
    {
        if (_items.Length >= min)
        {
            return;
        }

        var newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
        if (newCapacity < min)
        {
            newCapacity = min;
        }

        Capacity = newCapacity;
    }

    private static void ValidateAssignable(object? value, out T result)
    {
        if (!IsCompatibleObject(value))
        {
            throw new ArgumentException($"значение требуется типа {typeof(T)}.", nameof(value));
        }

        result = (T)value!;
    }

    private void ValidateCopyTo(T[] array, int arrayIndex)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (arrayIndex < 0 || arrayIndex > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        if (array.Length - arrayIndex < _count)
        {
            throw new ArgumentException("недостаточно места в массиве", nameof(array));
        }
    }

    private void EnsureIndexInRange(int index)
    {
        if ((uint)index >= (uint)_count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    private static bool IsCompatibleObject(object? value)
    {
        return value is null && default(T) is null || value is T;
    }

    public struct Enumerator : IEnumerator<T>
    {
        private readonly SimpleList<T> _list;
        private readonly int _version;
        private int _index;
        private T? _current;

        internal Enumerator(SimpleList<T> list)
        {
            _list = list;
            _version = list._version;
            _index = 0;
            _current = default;
        }

        public T Current => _current!;

        object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("коллекция была изменена во время enumeration обходв");
            }

            if ((uint)_index < (uint)_list._count)
            {
                _current = _list._items[_index++];
                return true;
            }

            _index = _list._count + 1;
            _current = default;
            return false;
        }

        public void Reset()
        {
            if (_version != _list._version)
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

    bool IList.IsReadOnly => IsReadOnly;
}