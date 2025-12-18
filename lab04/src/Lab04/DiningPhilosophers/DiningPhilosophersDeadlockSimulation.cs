using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab04.DiningPhilosophers;

public sealed class DiningPhilosophersDeadlockSimulation
{
    private readonly int _philosopherCount;
    private readonly SemaphoreSlim[] _forks;

    public DiningPhilosophersDeadlockSimulation(int philosopherCount = 5)
    {
        if (philosopherCount < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(philosopherCount), "количество философов должно быть не меньше двух");
        }

        _philosopherCount = philosopherCount;
        _forks = Enumerable.Range(0, philosopherCount)
            .Select(_ => new SemaphoreSlim(1, 1))
            .ToArray();
    }

    public async Task<bool> RunAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "нельзя поставтиь таймаут отрицательным!))!)!!(ы)");
        }

        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        using var barrier = new Barrier(_philosopherCount);
        var philosopherTasks = new Task[_philosopherCount];

        for (int i = 0; i < _philosopherCount; i++)
        {
            int philosopherId = i;
            philosopherTasks[i] = Task.Run(async () =>
            {
                var leftFork = _forks[philosopherId];
                var rightFork = _forks[(philosopherId + 1) % _philosopherCount];
                var token = combinedCts.Token;
                var leftAcquired = false;
                var rightAcquired = false;

                try
                {
                    barrier.SignalAndWait(token);

                    await leftFork.WaitAsync(token).ConfigureAwait(false);
                    leftAcquired = true;

                    await Task.Delay(10, token).ConfigureAwait(false);

                    await rightFork.WaitAsync(token).ConfigureAwait(false);
                    rightAcquired = true;

                    await Task.Delay(20, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // вдруг зависли задачи, должны их завершить путем отмены
                }
                finally
                {
                    if (rightAcquired)
                    {
                        rightFork.Release();
                    }

                    if (leftAcquired)
                    {
                        leftFork.Release();
                    }
                }
            }, combinedCts.Token);
        }

        var allPhilosophers = Task.WhenAll(philosopherTasks);
        var finished = await Task.WhenAny(allPhilosophers, Task.Delay(timeout, cancellationToken)).ConfigureAwait(false);

        if (finished == allPhilosophers && allPhilosophers.IsCompletedSuccessfully)
        {
            combinedCts.Cancel();
            return true;
        }

        combinedCts.Cancel();
        try
        {
            await Task.WhenAll(philosopherTasks).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Ingoring that podpishite moyi peticiu (OperationCanceledException)
        }

        return false;
    }
}