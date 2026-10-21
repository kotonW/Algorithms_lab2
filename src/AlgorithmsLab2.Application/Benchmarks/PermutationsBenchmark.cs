using AlgorithmsLab2.Domain.Permutations;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Бенчмарк перебора всех перестановок: сложность O(n!), максимум n = 15.</summary>
public sealed class PermutationsBenchmark : TimedAlgorithmBenchmark<int, long>
{
    /// <summary>Идентификатор алгоритма в настройках и реестре.</summary>
    public const string AlgorithmId = "permutations";

    /// <summary>Наибольшее n, допустимое в интерфейсе: 15! ≈ 1,3·10^12 — это уже часы работы.</summary>
    public const int MaxSupportedN = 15;

    public PermutationsBenchmark(AllPermutationsAlgorithm algorithm)
        : base(algorithm, CreateMetadata(), n => n, operations => operations)
    {
    }

    private static AlgorithmMetadata CreateMetadata() => new(
        Id: AlgorithmId,
        Name: "Перебор всех перестановок",
        Complexity: "O(n!)",
        GrowthLabel: "n!",
        Description: "Рекурсивный перебор с возвратом: n! перестановок чисел 1..n.",
        MinN: 1,
        MaxN: MaxSupportedN,
        Growth: n => (double)PermutationGenerator.Factorial(n),
        EstimatedMsPerUnit: 1.5e-5);
}
