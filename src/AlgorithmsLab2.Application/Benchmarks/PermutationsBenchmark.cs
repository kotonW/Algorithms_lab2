using AlgorithmsLab2.Domain.Benchmarks;
using AlgorithmsLab2.Domain.Permutations;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Бенчмарк перебора всех перестановок: сложность O(n!), максимум n = 15.</summary>
public sealed class PermutationsBenchmark : TimedAlgorithmBenchmark<int, long>, ITraceableBenchmark
{
    /// <summary>Идентификатор алгоритма в настройках и реестре.</summary>
    public const string AlgorithmId = "permutations";

    /// <summary>Наибольшее n, допустимое в интерфейсе: 15! ≈ 1,3·10^12 — это уже часы работы.</summary>
    public const int MaxSupportedN = 15;

    /// <summary>Предел числа узлов дерева рекурсии.</summary>
    private const int MaxTraceNodes = 400;

    public PermutationsBenchmark(AllPermutationsAlgorithm algorithm)
        : base(algorithm, CreateMetadata(), n => n, operations => operations)
    {
    }

    /// <inheritdoc />
    public int TraceDefaultN => 3;

    /// <inheritdoc />
    /// <remarks>При n = 5 в дереве 326 узлов, при n = 6 уже 1957.</remarks>
    public int TraceMaxN => 5;

    /// <inheritdoc />
    public RecursionTree BuildTrace(int n)
    {
        if (n < 1 || n > TraceMaxN)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, $"Для дерева n должно быть от 1 до {TraceMaxN}.");
        }

        return PermutationGenerator.Trace(n, MaxTraceNodes);
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
