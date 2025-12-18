using System.Collections;
using System.Collections.Generic;

namespace Lab03.Collections;

public class DoublyLinkedList<T> : IList<T>, IList, IReadOnlyList<T>
{
    private sealed class Node
    {
        public T Value;
        public Node? Next;
        public Node? Prev;

        public Node(T value) => Value = value;
    }

    private Node? _head;
    private Node? _tail;
    private int _count;
    private int _version;
    private object? _syncRoot;

    public int Count => _count;
    public bool IsReadOnly => false;

    bool IList.IsReadOnly => IsReadOnly;
    bool IList.IsFixedSize => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => _syncRoot ??= new object();

    public T this[int index]
    {
        get => GetNode(index).Value;
        set
        {
            var node = GetNode(index);
            node.Value = value;
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
        var node = new Node(item);
        if (_head is null)
        {
            _head = _tail = node;
        }
        else
        {
            _tail!.Next = node;
            node.Prev = _tail;
            _tail = node;
        }

        _count++;
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
        _head = _tail = null;
        _count = 0;
        _version++;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    bool IList.Contains(object? value) => IsCompatibleObject(value) && Contains((T)value!);

    public void CopyTo(T[] array, int arrayIndex)
    {
        ValidateCopyTo(array, arrayIndex);

        var current = _head;
        var idx = arrayIndex;
        while (current is not null)
        {
            array[idx++] = current.Value;
            current = current.Next;
        }
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

        var current = _head;
        var idx = index;
        while (current is not null)
        {
            array.SetValue(current.Value, idx++);
            current = current.Next;
        }
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        var current = _head;
        var index = 0;

        while (current is not null)
        {
            if (comparer.Equals(current.Value, item))
            {
                return index;
            }

            current = current.Next;
            index++;
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

        if (index == _count)
        {
            Add(item);
            return;
        }

        var existing = GetNode(index);
        var node = new Node(item)
        {
            Next = existing,
            Prev = existing.Prev
        };

        if (existing.Prev is null)
        {
            _head = node;
        }
        else
        {
            existing.Prev.Next = node;
        }

        existing.Prev = node;
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
        var node = FindNode(item);
        if (node is null)
        {
            return false;
        }

        RemoveNode(node);
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
        var node = GetNode(index);
        RemoveNode(node);
    }

    private void RemoveNode(Node node)
    {
        if (node.Prev is null)
        {
            _head = node.Next;
        }
        else
        {
            node.Prev.Next = node.Next;
        }

        if (node.Next is null)
        {
            _tail = node.Prev;
        }
        else
        {
            node.Next.Prev = node.Prev;
        }

        _count--;
        _version++;
    }

    private Node GetNode(int index)
    {
        if ((uint)index >= (uint)_count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (index < _count / 2)
        {
            var current = _head;
            for (var i = 0; i < index; i++)
            {
                current = current!.Next;
            }

            return current!;
        }
        else
        {
            var current = _tail;
            for (var i = _count - 1; i > index; i--)
            {
                current = current!.Prev;
            }

            return current!;
        }
    }

    private Node? FindNode(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        var current = _head;

        while (current is not null)
        {
            if (comparer.Equals(current.Value, item))
            {
                return current;
            }

            current = current.Next;
        }

        return null;
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

    private static void ValidateAssignable(object? value, out T typed)
    {
        if (!IsCompatibleObject(value))
        {
            throw new ArgumentException($"значение требуется типа {typeof(T)}.", nameof(value));
        }

        typed = (T)value!;
    }

    private static bool IsCompatibleObject(object? value)
    {
        return value is null && default(T) is null || value is T;
    }

    public struct Enumerator : IEnumerator<T>
    {
        private readonly DoublyLinkedList<T> _list;
        private readonly int _version;
        private Node? _current;
        private T? _currentValue;

        internal Enumerator(DoublyLinkedList<T> list)
        {
            _list = list;
            _version = list._version;
            _current = null;
            _currentValue = default;
        }

        public T Current => _currentValue!;

        object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("коллекция была изменена во время обхода");
            }

            _current = _current is null ? _list._head : _current.Next;
            if (_current is null)
            {
                _currentValue = default;
                return false;
            }

            _currentValue = _current.Value;
            return true;
        }

        public void Reset()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("коллекция была изменена во время обхода");
            }

            _current = null;
            _currentValue = default;
        }

        public void Dispose()
        {
        }
    }
}