using AlgorithmsLab2.Domain.Catalan;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>
/// Бенчмарк чисел Каталана наивной рекурсией: сложность O(3^n), максимум n = 20
/// (3^20 ≈ 3,5·10^9 вызовов — это уже десятки секунд на один прогон).
/// </summary>
public sealed class CatalanBenchmark : TimedAlgorithmBenchmark<int, CatalanResult>
{
    /// <summary>Идентификатор алгоритма в настройках и реестре.</summary>
    public const string AlgorithmId = "catalan";

    /// <summary>Наибольшее n, допустимое в интерфейсе.</summary>
    public const int MaxSupportedN = 20;

    public CatalanBenchmark(NaiveCatalanAlgorithm algorithm)
        : base(algorithm, CreateMetadata(), n => n, result => result.Calls)
    {
    }

    private static AlgorithmMetadata CreateMetadata() => new(
        Id: AlgorithmId,
        Name: "Числа Каталана (наивная рекурсия)",
        Complexity: "O(3^n)",
        GrowthLabel: "3^n",
        Description: "C(n) = Σ C(i)·C(n−1−i) без запоминания: подзадачи пересчитываются, вызовов ровно 3^n.",
        MinN: 1,
        MaxN: MaxSupportedN,
        Growth: n => Math.Pow(3, n),
        EstimatedMsPerUnit: 4e-6);
}
