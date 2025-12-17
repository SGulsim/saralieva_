using Lab03.Collections;
using NUnit.Framework;

namespace Lab03.Tests;

public class SimpleListTests
{
    [Test]
    public void Add_IncreasesCountAndStoresValues()
    {
        var list = new SimpleList<int>();
        list.Add(10);
        list.Add(20);

        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list[0], Is.EqualTo(10));
        Assert.That(list[1], Is.EqualTo(20));
    }

    [Test]
    public void Insert_ShiftsElements()
    {
        var list = new SimpleList<string> { "a", "b", "c" };
        list.Insert(1, "x");

        Assert.That(list.Count, Is.EqualTo(4));
        Assert.That(list[0], Is.EqualTo("a"));
        Assert.That(list[1], Is.EqualTo("x"));
        Assert.That(list[2], Is.EqualTo("b"));
        Assert.That(list[3], Is.EqualTo("c"));
    }

    [Test]
    public void Remove_ReturnsTrueAndShiftsTail()
    {
        var list = new SimpleList<int> { 1, 2, 3, 4 };
        var removed = list.Remove(3);

        Assert.That(removed, Is.True);
        Assert.That(list.Count, Is.EqualTo(3));
        Assert.That(list, Is.EqualTo(new[] { 1, 2, 4 }));
    }

    [Test]
    public void Enumerator_IteratesOverElements()
    {
        var list = new SimpleList<int> { 1, 2, 3 };
        var result = new List<int>();

        foreach (var value in list)
        {
            result.Add(value);
        }

        Assert.That(result, Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void Indexer_SetValue_ReplacesItem()
    {
        var list = new SimpleList<int> { 1, 2, 3 };
        list[1] = 42;

        Assert.That(list[1], Is.EqualTo(42));
        Assert.That(list, Is.EqualTo(new[] { 1, 42, 3 }));
    }

    [Test]
    public void Contains_ReturnsFalseWhenNotPresent()
    {
        var list = new SimpleList<int> { 1, 2, 3 };
        Assert.That(list.Contains(4), Is.False);
    }

    [Test]
    public void RemoveAt_InvalidIndex_Throws()
    {
        var list = new SimpleList<int> { 1, 2, 3 };
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(3));
    }
}