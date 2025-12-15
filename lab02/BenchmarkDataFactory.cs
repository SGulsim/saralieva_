using System.Collections.Immutable;

namespace Lab02;
// Создаёт коллекции с начальным набором данных (списки, очереди, стеки, immutable-списки, связные списки)
public static class BenchmarkDataFactory
{
    public static List<int> CreateList(int count) =>
        Enumerable.Range(0, count).ToList();

    public static LinkedList<int> CreateLinkedList(int count)
    {
        var linked = new LinkedList<int>();
        foreach (var value in Enumerable.Range(0, count))
        {
            linked.AddLast(value);
        }

        return linked;
    }

    public static Queue<int> CreateQueue(int count) =>
        new Queue<int>(Enumerable.Range(0, count));

    public static Stack<int> CreateStack(int count) =>
        new Stack<int>(Enumerable.Range(0, count));

    public static ImmutableList<int> CreateImmutableList(int count) =>
        ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, count));
}