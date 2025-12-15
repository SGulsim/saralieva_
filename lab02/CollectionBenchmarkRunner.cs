using System.Collections.Immutable;
using System.Diagnostics;

namespace Lab02;
// Основная программа — вызывает тесты для каждой коллекции и показывает результаты
// CollectionBenchmarkSummary - описывает итог по конкретной коллекции: название и список результатов операций
// OperationResult - структура с названием операции, средним временем (или null, если неприменимо) и флагом применимости
public static class CollectionBenchmarkRunner
{
    // Каждый метод Benchmark возвращает CollectionBenchmarkSummary, где упакован список OperationResult
    public static IReadOnlyList<CollectionBenchmarkSummary> Run(BenchmarkConfiguration config)
    {
        var results = new List<CollectionBenchmarkSummary>
        {
            BenchmarkList(config),
            BenchmarkLinkedList(config),
            BenchmarkQueue(config),
            BenchmarkStack(config),
            BenchmarkImmutableList(config)
        };

        return results;
    }

    private static CollectionBenchmarkSummary BenchmarkList(BenchmarkConfiguration config)
    {
        var operations = new List<OperationResult>
        {
            Measure("Добавление в конец", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list.Add(next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Добавление в начало", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list.Insert(0, next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Добавление в середину", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list.Insert(list.Count / 2, next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление из начала", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list.RemoveAt(0);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление с конца", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list.RemoveAt(list.Count - 1);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление из середины", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list.RemoveAt(list.Count / 2);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Поиск по значению", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateList(config.InitialSize);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list.Contains(config.SearchValue);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Доступ по индексу", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateList(config.InitialSize);
                var index = config.IndexForAccess;
                var sw = Stopwatch.StartNew();
                int value = 0;
                for (int i = 0; i < config.OperationCount; i++)
                {
                    value = list[index];
                }

                sw.Stop();
                GC.KeepAlive(value);
                return sw.Elapsed.TotalMilliseconds;
            })
        };

        return new CollectionBenchmarkSummary("List<int>", operations);
    }

    private static CollectionBenchmarkSummary BenchmarkLinkedList(BenchmarkConfiguration config)
    {
        var operations = new List<OperationResult>
        {
            Measure("Добавление в конец", config, _ =>
            {
                var linked = BenchmarkDataFactory.CreateLinkedList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    linked.AddLast(next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Добавление в начало", config, _ =>
            {
                var linked = BenchmarkDataFactory.CreateLinkedList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    linked.AddFirst(next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Добавление в середину", config, _ =>
            {
                var linked = BenchmarkDataFactory.CreateLinkedList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    var middleNode = GetNodeAt(linked, linked.Count / 2);
                    linked.AddBefore(middleNode, next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление из начала", config, _ =>
            {
                var linked = BenchmarkDataFactory.CreateLinkedList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    linked.RemoveFirst();
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление с конца", config, _ =>
            {
                var linked = BenchmarkDataFactory.CreateLinkedList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    linked.RemoveLast();
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление из середины", config, _ =>
            {
                var linked = BenchmarkDataFactory.CreateLinkedList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    var middleNode = GetNodeAt(linked, linked.Count / 2);
                    linked.Remove(middleNode);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Поиск по значению", config, _ =>
            {
                var linked = BenchmarkDataFactory.CreateLinkedList(config.InitialSize);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    linked.Find(config.SearchValue);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Доступ по индексу (итерация)", config, _ =>
            {
                var linked = BenchmarkDataFactory.CreateLinkedList(config.InitialSize);
                int value = 0;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    value = GetNodeAt(linked, config.IndexForAccess).Value;
                }

                sw.Stop();
                GC.KeepAlive(value);
                return sw.Elapsed.TotalMilliseconds;
            })
        };

        return new CollectionBenchmarkSummary("LinkedList<int>", operations);
    }

    private static CollectionBenchmarkSummary BenchmarkQueue(BenchmarkConfiguration config)
    {
        var operations = new List<OperationResult>
        {
            Measure("Добавление (enqueue)", config, _ =>
            {
                var queue = BenchmarkDataFactory.CreateQueue(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    queue.Enqueue(next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            NotApplicable("Добавление в начало"),
            NotApplicable("Добавление в середину"),
            Measure("Удаление из начала (dequeue)", config, _ =>
            {
                var queue = BenchmarkDataFactory.CreateQueue(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    queue.Dequeue();
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            NotApplicable("Удаление с конца"),
            NotApplicable("Удаление из середины"),
            Measure("Поиск по значению", config, _ =>
            {
                var queue = BenchmarkDataFactory.CreateQueue(config.InitialSize);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    queue.Contains(config.SearchValue);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            NotApplicable("Доступ по индексу")
        };

        return new CollectionBenchmarkSummary("Queue<int>", operations);
    }

    private static CollectionBenchmarkSummary BenchmarkStack(BenchmarkConfiguration config)
    {
        var operations = new List<OperationResult>
        {
            Measure("Добавление (push)", config, _ =>
            {
                var stack = BenchmarkDataFactory.CreateStack(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    stack.Push(next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            NotApplicable("Добавление в начало"),
            NotApplicable("Добавление в середину"),
            Measure("Удаление (pop)", config, _ =>
            {
                var stack = BenchmarkDataFactory.CreateStack(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    stack.Pop();
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            NotApplicable("Удаление с конца"),
            NotApplicable("Удаление из середины"),
            Measure("Поиск по значению", config, _ =>
            {
                var stack = BenchmarkDataFactory.CreateStack(config.InitialSize);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    stack.Contains(config.SearchValue);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            NotApplicable("Доступ по индексу")
        };

        return new CollectionBenchmarkSummary("Stack<int>", operations);
    }

    private static CollectionBenchmarkSummary BenchmarkImmutableList(BenchmarkConfiguration config)
    {
        var operations = new List<OperationResult>
        {
            Measure("Добавление в конец", config, _ =>
            {
                ImmutableList<int> list = BenchmarkDataFactory.CreateImmutableList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list = list.Add(next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Добавление в начало", config, _ =>
            {
                ImmutableList<int> list = BenchmarkDataFactory.CreateImmutableList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list = list.Insert(0, next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Добавление в середину", config, _ =>
            {
                ImmutableList<int> list = BenchmarkDataFactory.CreateImmutableList(config.InitialSize);
                int next = config.InitialSize;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list = list.Insert(list.Count / 2, next++);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление из начала", config, _ =>
            {
                ImmutableList<int> list = BenchmarkDataFactory.CreateImmutableList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list = list.RemoveAt(0);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление с конца", config, _ =>
            {
                ImmutableList<int> list = BenchmarkDataFactory.CreateImmutableList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list = list.RemoveAt(list.Count - 1);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Удаление из середины", config, _ =>
            {
                ImmutableList<int> list = BenchmarkDataFactory.CreateImmutableList(config.InitialSize + config.OperationCount);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list = list.RemoveAt(list.Count / 2);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Поиск по значению", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateImmutableList(config.InitialSize);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    list.Contains(config.SearchValue);
                }

                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            }),
            Measure("Доступ по индексу", config, _ =>
            {
                var list = BenchmarkDataFactory.CreateImmutableList(config.InitialSize);
                int value = 0;
                var index = config.IndexForAccess;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < config.OperationCount; i++)
                {
                    value = list[index];
                }

                sw.Stop();
                GC.KeepAlive(value);
                return sw.Elapsed.TotalMilliseconds;
            })
        };

        return new CollectionBenchmarkSummary("ImmutableList<int>", operations);
    }

    // Measure получает функцию и ее описание операции, возвращает operationResult 
    private static OperationResult Measure(
        string name,
        BenchmarkConfiguration config,
        Func<int, double> measureRun,
        bool applicable = true,
        string? notes = null)
    {
        if (!applicable)
        {
            return new OperationResult(name, null, false, notes);
        }

        double total = 0;
        for (int run = 0; run < config.Runs; run++)
        {
            total += measureRun(run);
        }

        return new OperationResult(name, total / config.Runs, true, notes);
    }

    private static OperationResult NotApplicable(string operation, string? notes = null) =>
        new(operation, null, false, notes ?? "Операция не поддерживается данной коллекцией");

    private static LinkedListNode<int> GetNodeAt(LinkedList<int> list, int index)
    {
        if (index < 0 || index >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var node = list.First;
        for (int i = 0; i < index; i++)
        {
            node = node!.Next;
        }

        return node!;
    }
}