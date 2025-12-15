using NUnit.Framework;

namespace Lab02.Tests;

[TestFixture]
public sealed class QueueCollectionTests
{
    [Test]
    public void Enqueue_ShouldAppendElement()
    {
        var queue = BenchmarkDataFactory.CreateQueue(3);
        queue.Enqueue(99);

        Assert.That(queue.Count, Is.EqualTo(4));
        Assert.That(queue.Contains(99), Is.True);
    }

    [Test]
    public void Dequeue_ShouldReturnOldestElement()
    {
        var queue = BenchmarkDataFactory.CreateQueue(3);
        var dequeued = queue.Dequeue();

        Assert.That(dequeued, Is.EqualTo(0));
        Assert.That(queue.Count, Is.EqualTo(2));
    }

    [Test]
    public void Contains_ShouldFindExistingElement()
    {
        var queue = BenchmarkDataFactory.CreateQueue(5);
        Assert.That(queue.Contains(4), Is.True);
    }
}