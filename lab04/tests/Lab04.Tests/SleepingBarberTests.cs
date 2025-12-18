using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lab04.SleepingBarber;
using Xunit;

namespace Lab04.Tests;

public class SleepingBarberTests
{
    [Fact]
    public async Task SleepingBarber_ShouldServeCustomersWithinCapacity()
    {
        var shop = new BarberShop(waitingChairs: 2);
        using var barberCts = new CancellationTokenSource();
        var servedCounter = 0;

        var barberTask = Task.Run(() => shop.RunBarberAsync(async customerId =>
        {
            Interlocked.Increment(ref servedCounter);
            await Task.Delay(20);
        }, barberCts.Token));

        const int totalCustomers = 10;
        var servedCustomers = new ConcurrentBag<int>();
        var turnedAwayCustomers = new ConcurrentBag<int>();

        var customerTasks = Enumerable.Range(0, totalCustomers).Select(async id =>
        {
            try
            {
                if (await shop.TryEnterAsync(id))
                {
                    servedCustomers.Add(id);
                }
                else
                {
                    turnedAwayCustomers.Add(id);
                }
            }
            catch (OperationCanceledException)
            {
                turnedAwayCustomers.Add(id);
            }
        });

        await Task.WhenAll(customerTasks);
        await Task.Delay(500);

        barberCts.Cancel();

        try
        {
            await barberTask;
        }
        catch (OperationCanceledException)
        {
            // ждем отмену для завершения цикла
        }

        Assert.Equal(servedCustomers.Count, Volatile.Read(ref servedCounter));
        Assert.Equal(totalCustomers, servedCustomers.Count + turnedAwayCustomers.Count);
    }
}