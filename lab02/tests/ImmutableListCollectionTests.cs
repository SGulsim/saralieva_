using NUnit.Framework;
using System.Collections.Immutable;

namespace Lab02.Tests;

[TestFixture]
public sealed class ImmutableListCollectionTests
{
    [Test]
    public void Add_ShouldReturnNewInstance()
    {
        var list = BenchmarkDataFactory.CreateImmutableList(3);
        var updated = list.Add(99);

        Assert.That(list.Count, Is.EqualTo(3));
        Assert.That(updated.Count, Is.EqualTo(4));
        Assert.That(updated[^1], Is.EqualTo(99));
    }

    [Test]
    public void InsertAtStart_ShouldNotAffectOriginalList()
    {
        var list = BenchmarkDataFactory.CreateImmutableList(3);
        var updated = list.Insert(0, -1);

        Assert.That(list[0], Is.EqualTo(0));
        Assert.That(updated[0], Is.EqualTo(-1));
    }

    [Test]
    public void RemoveAt_ShouldReturnListWithoutElement()
    {
        var list = BenchmarkDataFactory.CreateImmutableList(5);
        var updated = list.RemoveAt(2);

        Assert.That(list.Count, Is.EqualTo(5));
        Assert.That(updated.Count, Is.EqualTo(4));
        Assert.That(updated.Contains(2), Is.False);
    }

    [Test]
    public void Contains_ShouldFindExistingValue()
    {
        var list = BenchmarkDataFactory.CreateImmutableList(5);
        Assert.That(list.Contains(4), Is.True);
    }

    [Test]
    public void Indexer_ShouldReturnCorrectValue()
    {
        var list = BenchmarkDataFactory.CreateImmutableList(5);
        Assert.That(list[3], Is.EqualTo(3));
    }
}