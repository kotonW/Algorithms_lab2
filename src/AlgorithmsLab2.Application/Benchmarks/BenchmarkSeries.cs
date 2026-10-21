namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Результат эксперимента: описание алгоритма и точки по возрастанию n.</summary>
public sealed record BenchmarkSeries(AlgorithmMetadata Metadata, IReadOnlyList<BenchmarkPoint> Points)
{
    /// <summary>Пустой ряд — график без данных.</summary>
    public static BenchmarkSeries Empty(AlgorithmMetadata metadata) => new(metadata, []);
}
