namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Алгоритм, который можно замерять: описание и один прогон на заданном n.</summary>
public interface IAlgorithmBenchmark
{
    /// <summary>Описание алгоритма.</summary>
    AlgorithmMetadata Metadata { get; }

    /// <summary>
    /// Выполняет один прогон на размере <paramref name="n"/> и возвращает число выполненных операций
    /// (для перестановок — n!). Время замеряет вызывающий код.
    /// </summary>
    long RunOnce(int n, CancellationToken cancellationToken);
}
