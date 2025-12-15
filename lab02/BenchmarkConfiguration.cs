namespace Lab02;
// Хранит параметры запуска: InitialSize, OperationCount, Runs, SearchValue, IndexForAccess и т.п
public sealed record BenchmarkConfiguration(
    int InitialSize,
    int OperationCount,
    int Runs,
    int SearchValue,
    int IndexForAccess)
{
    public static BenchmarkConfiguration Default =>
        new(
            InitialSize: 100_000,
            OperationCount: 10_000,
            Runs: 5,
            SearchValue: 75_000,
            IndexForAccess: 50_000);
}