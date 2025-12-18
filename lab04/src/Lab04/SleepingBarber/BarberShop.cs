using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab04.SleepingBarber;

public sealed class BarberShop
{
    private readonly int _waitingRoomCapacity;
    private readonly SemaphoreSlim _customers = new(0);
    private readonly Queue<CustomerRequest> _queue = new();
    private readonly object _queueLock = new();
    private bool _barberBusy;

    public BarberShop(int waitingChairs)
    {
        if (waitingChairs < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(waitingChairs), "количество стульев должно быть положительным емае");
        }

        _waitingRoomCapacity = waitingChairs;
    }

    public async Task<bool> TryEnterAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var request = new CustomerRequest(customerId);
        var accepted = false;

        lock (_queueLock)
        {
            if (!_barberBusy)
            {
                _barberBusy = true;
                _queue.Enqueue(request);
                accepted = true;
            }
            else if (_queue.Count < _waitingRoomCapacity)
            {
                _queue.Enqueue(request);
                accepted = true;
            }
        }

        if (!accepted)
        {
            return false;
        }

        _customers.Release();
        await request.Completion.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task RunBarberAsync(Func<int, Task> service, CancellationToken cancellationToken = default)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        try
        {
            while (true)
            {
                await _customers.WaitAsync(cancellationToken).ConfigureAwait(false);

                CustomerRequest request;
                lock (_queueLock)
                {
                    request = _queue.Dequeue();
                }

                await service(request.CustomerId).WaitAsync(cancellationToken).ConfigureAwait(false);
                request.Completion.TrySetResult(true);

                lock (_queueLock)
                {
                    _barberBusy = _queue.Count > 0;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // завершаем работу парикмахера в штатном режиме
        }
        finally
        {
            lock (_queueLock)
            {
                while (_queue.Count > 0)
                {
                    var pending = _queue.Dequeue();
                    pending.Completion.TrySetCanceled(cancellationToken);
                }

                _barberBusy = false;
            }
        }
    }

    private sealed class CustomerRequest
    {
        public CustomerRequest(int customerId)
        {
            CustomerId = customerId;
            Completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public int CustomerId { get; }

        public TaskCompletionSource<bool> Completion { get; }
    }
}