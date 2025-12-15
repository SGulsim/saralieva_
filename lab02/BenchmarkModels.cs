namespace Lab02;

public sealed record OperationResult(
    string Operation,
    double? AverageMilliseconds,
    bool Applicable,
    string? Notes = null);

public sealed record CollectionBenchmarkSummary(
    string CollectionName,
    IReadOnlyList<OperationResult> Operations);