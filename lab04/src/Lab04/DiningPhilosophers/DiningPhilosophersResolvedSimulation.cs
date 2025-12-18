using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab04.DiningPhilosophers;

public sealed class DiningPhilosophersResolvedSimulation
{
    private readonly int _philosopherCount;
    private readonly SemaphoreSlim[] _forks;
    private readonly int[] _meals;
    private readonly IReadOnlyList<int> _mealsView;

    public DiningPhilosophersResolvedSimulation(int philosopherCount = 5)
    {
        if (philosopherCount < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(philosopherCount), "количество философов должно быть не меньше двух");
        }

        _philosopherCount = philosopherCount;
        _forks = Enumerable.Range(0, philosopherCount)
            .Select(_ => new SemaphoreSlim(1, 1))
            .ToArray();

        _meals = new int[_philosopherCount];
        _mealsView = Array.AsReadOnly(_meals);
    }

    public IReadOnlyList<int> MealsEaten => _mealsView;

    public async Task RunAsync(int mealsPerPhilosopher, CancellationToken cancellationToken = default)
    {
        if (mealsPerPhilosopher <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(mealsPerPhilosopher), "философы должны покушать не меньше одного раза...");
        }

        Array.Fill(_meals, 0);
        var simulationTasks = new Task[_philosopherCount];

        for (int i = 0; i < _philosopherCount; i++)
        {
            int philosopherId = i;
            simulationTasks[i] = Task.Run(async () =>
            {
                var leftIndex = philosopherId;
                var rightIndex = (philosopherId + 1) % _philosopherCount;
                var firstForkIndex = Math.Min(leftIndex, rightIndex);
                var secondForkIndex = Math.Max(leftIndex, rightIndex);

                for (int meal = 0; meal < mealsPerPhilosopher; meal++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await ThinkAsync(cancellationToken).ConfigureAwait(false);

                    await _forks[firstForkIndex].WaitAsync(cancellationToken).ConfigureAwait(false);
                    await _forks[secondForkIndex].WaitAsync(cancellationToken).ConfigureAwait(false);

                    await EatAsync(cancellationToken).ConfigureAwait(false);
                    Interlocked.Increment(ref _meals[philosopherId]);

                    _forks[secondForkIndex].Release();
                    _forks[firstForkIndex].Release();

                    await PauseAsync(cancellationToken).ConfigureAwait(false);
                }
            }, cancellationToken);
        }

        await Task.WhenAll(simulationTasks).ConfigureAwait(false);
    }

    private static Task ThinkAsync(CancellationToken token) =>
        Task.Delay(Random.Shared.Next(5, 15), token);

    private static Task EatAsync(CancellationToken token) =>
        Task.Delay(Random.Shared.Next(5, 15), token);

    private static Task PauseAsync(CancellationToken token) =>
        Task.Delay(Random.Shared.Next(5, 15), token);
}