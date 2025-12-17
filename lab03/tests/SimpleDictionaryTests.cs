using Lab03.Collections;
using NUnit.Framework;

namespace Lab03.Tests;

public class SimpleDictionaryTests
{
    [Test]
    public void Add_StoresValuesAndCanBeRetrieved()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);

        Assert.That(dict.Count, Is.EqualTo(2));
        Assert.That(dict["one"], Is.EqualTo(1));
        Assert.That(dict["two"], Is.EqualTo(2));
    }

    [Test]
    public void Add_DuplicateKey_Throws()
    {
        var dict = new SimpleDictionary<int, string>();
        dict.Add(1, "a");

        Assert.Throws<ArgumentException>(() => dict.Add(1, "b"));
    }

    [Test]
    public void Indexer_Set_UpdatesValue()
    {
        var dict = new SimpleDictionary<int, string>();
        dict[1] = "alpha";
        dict[1] = "beta";

        Assert.That(dict[1], Is.EqualTo("beta"));
    }

    [Test]
    public void Remove_ReturnsTrueAndDeletesKey()
    {
        var dict = new SimpleDictionary<string, int>
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3
        };

        var removed = dict.Remove("b");
        Assert.That(removed, Is.True);
        Assert.That(dict.Count, Is.EqualTo(2));
        Assert.That(dict.ContainsKey("b"), Is.False);
    }

    [Test]
    public void TryGetValue_ReturnsFalseWhenMissing()
    {
        var dict = new SimpleDictionary<int, string>();
        var found = dict.TryGetValue(10, out var value);

        Assert.That(found, Is.False);
        Assert.That(value, Is.Null);
    }

    [Test]
    public void Enumeration_EnumeratesPairs()
    {
        var dict = new SimpleDictionary<string, int>
        {
            ["x"] = 1,
            ["y"] = 2
        };

        var pairs = dict.ToList();

        Assert.That(pairs, Has.Count.EqualTo(2));
        Assert.That(
            pairs,
            Is.EquivalentTo(new[]
            {
                new KeyValuePair<string, int>("x", 1),
                new KeyValuePair<string, int>("y", 2)
            }));
    }
}