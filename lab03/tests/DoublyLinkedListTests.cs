using Lab03.Collections;
using NUnit.Framework;

namespace Lab03.Tests;

public class DoublyLinkedListTests
{
    [Test]
    public void Add_AppendsToTailAndCountIncreases()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        Assert.That(list, Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(list.Count, Is.EqualTo(3));
    }

    [Test]
    public void Insert_AtHead_ShiftsExistingNodes()
    {
        var list = new DoublyLinkedList<string>();
        list.Add("b");
        list.Add("c");

        list.Insert(0, "a");

        Assert.That(list, Is.EqualTo(new[] { "a", "b", "c" }));
    }

    [Test]
    public void Insert_InMiddle_SplitsList()
    {
        var list = new DoublyLinkedList<string>();
        list.Add("a");
        list.Add("c");

        list.Insert(1, "b");

        Assert.That(list, Is.EqualTo(new[] { "a", "b", "c" }));
    }

    [Test]
    public void Remove_RemovesFirstOccurrence()
    {
        var list = new DoublyLinkedList<int> { 1, 2, 3, 2 };

        var removed = list.Remove(2);

        Assert.That(removed, Is.True);
        Assert.That(list, Is.EqualTo(new[] { 1, 3, 2 }));
    }

    [Test]
    public void RemoveAt_ByIndex()
    {
        var list = new DoublyLinkedList<int> { 10, 20, 30 };

        list.RemoveAt(1);

        Assert.That(list, Is.EqualTo(new[] { 10, 30 }));
    }

    [Test]
    public void Indexer_ReadsAndWrites()
    {
        var list = new DoublyLinkedList<int> { 5, 6, 7 };

        Assert.That(list[2], Is.EqualTo(7));

        list[1] = 42;

        Assert.That(list, Is.EqualTo(new[] { 5, 42, 7 }));
    }

    [Test]
    public void Contains_ReturnsFalseWhenAbsent()
    {
        var list = new DoublyLinkedList<int> { 1, 2, 3 };

        Assert.That(list.Contains(4), Is.False);
    }

    [Test]
    public void Enumerator_IteratesCorrectly()
    {
        var list = new DoublyLinkedList<int> { 1, 2, 3 };
        var collected = new List<int>();

        foreach (var value in list)
        {
            collected.Add(value);
        }

        Assert.That(collected, Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void RemoveAt_InvalidIndex_Throws()
    {
        var list = new DoublyLinkedList<int> { 1, 2, 3 };

        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(3));
    }
}