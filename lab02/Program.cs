using Lab02;

using NUnitLite;
using System.Linq;

namespace Lab02;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Contains("--tests", StringComparer.OrdinalIgnoreCase))
        {
            var filteredArgs = args.Where(a => !string.Equals(a, "--tests", StringComparison.OrdinalIgnoreCase)).ToArray();
            return new AutoRun(typeof(Program).Assembly).Execute(filteredArgs);
        }

        var config = BenchmarkConfiguration.Default;
        var summaries = CollectionBenchmarkRunner.Run(config);
        BenchmarkPrinter.Print(summaries, config);

        Console.WriteLine("\nДля запуска тестов выполните: dotnet run -- --tests");
        return 0;
    }
}