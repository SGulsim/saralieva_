using System.Globalization;
using System.Text;

namespace Lab02;

public static class BenchmarkPrinter
{
    public static void Print(IEnumerable<CollectionBenchmarkSummary> summaries, BenchmarkConfiguration config)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var culture = CultureInfo.InvariantCulture;

        Console.WriteLine("=== Замеры производительности коллекций ===");
        Console.WriteLine($"Размер исходных коллекций: {config.InitialSize:N0}");
        Console.WriteLine($"Количество повторов операции: {config.OperationCount:N0}");
        Console.WriteLine($"Число прогонов: {config.Runs}");
        Console.WriteLine();

        foreach (var summary in summaries)
        {
            Console.WriteLine(summary.CollectionName);
            Console.WriteLine(new string('-', summary.CollectionName.Length));

            Console.WriteLine($"{"Операция",-40} | {"Среднее время, мс",20}");
            Console.WriteLine(new string('-', 40) + "-+-" + new string('-', 20));

            foreach (var operation in summary.Operations)
            {
                var value = operation.Applicable
                    ? operation.AverageMilliseconds!.Value.ToString("F2", culture)
                    : "N/A";

                Console.WriteLine($"{operation.Operation,-40} | {value,20}");

                if (!string.IsNullOrWhiteSpace(operation.Notes))
                {
                    Console.WriteLine($"  Примечание: {operation.Notes}");
                }
            }

            Console.WriteLine();
        }
    }
}