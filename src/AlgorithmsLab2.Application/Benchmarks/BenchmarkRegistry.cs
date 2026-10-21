namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Список доступных для замера алгоритмов. Наполняется через DI.</summary>
public sealed class BenchmarkRegistry
{
    private readonly List<IAlgorithmBenchmark> _algorithms;

    public BenchmarkRegistry(IEnumerable<IAlgorithmBenchmark> algorithms)
    {
        ArgumentNullException.ThrowIfNull(algorithms);

        _algorithms = algorithms.ToList();

        if (_algorithms.Count == 0)
        {
            throw new ArgumentException("Нужен хотя бы один алгоритм для замеров.", nameof(algorithms));
        }

        var duplicate = _algorithms
            .GroupBy(algorithm => algorithm.Metadata.Id, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate is not null)
        {
            throw new ArgumentException($"Алгоритм с идентификатором «{duplicate.Key}» зарегистрирован дважды.", nameof(algorithms));
        }
    }

    /// <summary>Все алгоритмы в порядке регистрации.</summary>
    public IReadOnlyList<IAlgorithmBenchmark> Algorithms => _algorithms;

    /// <summary>Алгоритм по умолчанию — первый зарегистрированный.</summary>
    public IAlgorithmBenchmark Default => _algorithms[0];

    /// <summary>Ищет алгоритм по идентификатору; <c>null</c>, если такого нет.</summary>
    public IAlgorithmBenchmark? Find(string? id) =>
        _algorithms.FirstOrDefault(algorithm => string.Equals(algorithm.Metadata.Id, id, StringComparison.Ordinal));
}
