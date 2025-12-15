using NUnit.Framework;

namespace Lab02.Tests;

[TestFixture]
public sealed class StackCollectionTests
{
    [Test]
    public void Push_ShouldIncreaseCount()
    {
        var stack = BenchmarkDataFactory.CreateStack(3);
        stack.Push(99);

        Assert.That(stack.Count, Is.EqualTo(4));
    }

    [Test]
    public void Pop_ShouldReturnLastPushed()
    {
        var stack = BenchmarkDataFactory.CreateStack(3);
        stack.Push(99);
        var popped = stack.Pop();

        Assert.That(popped, Is.EqualTo(99));
        Assert.That(stack.Count, Is.EqualTo(3));
    }

    [Test]
    public void Contains_ShouldFindExistingElement()
    {
        var stack = BenchmarkDataFactory.CreateStack(5);
        Assert.That(stack.Contains(2), Is.True);
    }
}