using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Lab04.ProducerConsumer;
using Xunit;

namespace Lab04.Tests;

public class ProducerConsumerTests
{
    [Fact]
    public async Task ProducerConsumer_ShouldTransferAllItems()
    {
        await using var buffer = new ProducerConsumerQueue<int>(capacity: 5);

        var produced = new ConcurrentBag<int>();
        var consumed = new ConcurrentBag<int>();

        var producerTasks = Enumerable.Range(0, 4).Select(producerId => Task.Run(async () =>
        {
            for (int i = 0; i < 25; i++)
            {
                int value = producerId * 100 + i;
                await buffer.AddAsync(value);
                produced.Add(value);
                await Task.Delay(2);
            }
        }));

        var consumerTasks = Enumerable.Range(0, 4).Select(_ => Task.Run(async () =>
        {
            for (int i = 0; i < 25; i++)
            {
                var item = await buffer.TakeAsync();
                consumed.Add(item);
                await Task.Delay(2);
            }
        }));

        await Task.WhenAll(producerTasks);
        buffer.CompleteAdding();
        await Task.WhenAll(consumerTasks);

        Assert.Equal(produced.Count, consumed.Count);
        Assert.Empty(produced.Except(consumed));
    }

    [Fact]
    public async Task ProducerConsumer_ShouldThrowAfterCompletion()
    {
        await using var buffer = new ProducerConsumerQueue<int>(capacity: 1);
        buffer.CompleteAdding();
        await Assert.ThrowsAsync<InvalidOperationException>(() => buffer.TakeAsync());
    }
}