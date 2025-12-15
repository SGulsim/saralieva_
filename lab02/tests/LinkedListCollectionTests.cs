using NUnit.Framework;

namespace Lab02.Tests;

[TestFixture]
public sealed class LinkedListCollectionTests
{
    [Test]
    public void AddFirst_ShouldAddElementToHead()
    {
        var linked = BenchmarkDataFactory.CreateLinkedList(3);
        linked.AddFirst(-1);

        Assert.That(linked.First!.Value, Is.EqualTo(-1));
    }

    [Test]
    public void AddLast_ShouldAddElementToTail()
    {
        var linked = BenchmarkDataFactory.CreateLinkedList(3);
        linked.AddLast(99);

        Assert.That(linked.Last!.Value, Is.EqualTo(99));
    }

    [Test]
    public void RemoveFirst_ShouldRemoveHead()
    {
        var linked = BenchmarkDataFactory.CreateLinkedList(3);
        linked.RemoveFirst();

        Assert.That(linked.First!.Value, Is.EqualTo(1));
        Assert.That(linked.Count, Is.EqualTo(2));
    }

    [Test]
    public void RemoveMiddle_ShouldRemoveCorrectNode()
    {
        var linked = BenchmarkDataFactory.CreateLinkedList(5);
        var middle = linked.Find(2)!;
        linked.Remove(middle);

        Assert.That(linked.Count, Is.EqualTo(4));
        Assert.That(linked.Contains(2), Is.False);
    }

    [Test]
    public void Find_ShouldLocateExistingValue()
    {
        var linked = BenchmarkDataFactory.CreateLinkedList(5);
        Assert.That(linked.Find(3), Is.Not.Null);
    }
}