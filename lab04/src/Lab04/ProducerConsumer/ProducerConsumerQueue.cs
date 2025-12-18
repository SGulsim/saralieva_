using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab04.ProducerConsumer;

public sealed class ProducerConsumerQueue<T> : IAsyncDisposable
{
    private readonly Queue<T> _queue = new();
    private readonly SemaphoreSlim _itemsAvailable = new(0);
    private readonly SemaphoreSlim _slotsAvailable;
    private readonly object _syncRoot = new();
    private bool _isAddingCompleted;
    private bool _disposed;
    private int _waitingConsumers;

    public ProducerConsumerQueue(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "размер буфера должен быть положительным если что");
        }

        Capacity = capacity;
        _slotsAvailable = new SemaphoreSlim(capacity, capacity);
    }

    public int Capacity { get; }

    public async Task AddAsync(T item, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _slotsAvailable.WaitAsync(cancellationToken).ConfigureAwait(false);

        var shouldSignal = false;

        lock (_syncRoot)
        {
            if (_isAddingCompleted)
            {
                _slotsAvailable.Release();
                throw new InvalidOperationException("добавили все элементы, произвели release()");
            }

            _queue.Enqueue(item);
            shouldSignal = Volatile.Read(ref _waitingConsumers) > 0;
        }

        if (shouldSignal)
        {
            _itemsAvailable.Release();
        }
    }

    public async Task<T> TakeAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        Interlocked.Increment(ref _waitingConsumers);

        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                lock (_syncRoot)
                {
                    if (_queue.Count > 0)
                    {
                        var item = _queue.Dequeue();
                        _slotsAvailable.Release();
                        return item;
                    }

                    if (_isAddingCompleted)
                    {
                        throw new InvalidOperationException("емкость буфера пуста, закрываем для чтения");
                    }
                }

                await _itemsAvailable.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            Interlocked.Decrement(ref _waitingConsumers);
        }
    }

    public void CompleteAdding()
    {
        ThrowIfDisposed();

        int waitersToWake = 0;

        lock (_syncRoot)
        {
            if (_isAddingCompleted)
            {
                return;
            }

            _isAddingCompleted = true;

            if (_queue.Count == 0)
            {
                waitersToWake = Math.Max(Volatile.Read(ref _waitingConsumers), 0);
            }
        }

        if (waitersToWake > 0)
        {
            _itemsAvailable.Release(waitersToWake);
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ProducerConsumerQueue<T>));
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _itemsAvailable.Dispose();
        _slotsAvailable.Dispose();
        await Task.CompletedTask;
    }
}