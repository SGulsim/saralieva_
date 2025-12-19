using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lab04.DiningPhilosophers;
using Xunit;

namespace Lab04.Tests;

public class DiningPhilosophersTests
{
    [Fact]
    public async Task DeadlockSimulation_ShouldDetectDeadlock()
    {
        var simulation = new DiningPhilosophersDeadlockSimulation();
        var completed = await simulation.RunAsync(TimeSpan.FromMilliseconds(500));
        Assert.False(completed);
    }

    [Fact]
    public async Task ResolvedSimulation_ShouldAvoidDeadlock()
    {
        var simulation = new DiningPhilosophersResolvedSimulation();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        await simulation.RunAsync(mealsPerPhilosopher: 3, cts.Token);

        Assert.All(simulation.MealsEaten, meals => Assert.Equal(3, meals));
    }
}