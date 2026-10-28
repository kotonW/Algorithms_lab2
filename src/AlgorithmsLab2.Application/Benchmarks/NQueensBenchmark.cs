using AlgorithmsLab2.Domain.Benchmarks;
using AlgorithmsLab2.Domain.Permutations;
using AlgorithmsLab2.Domain.Queens;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>
/// Бенчмарк расстановки N ферзей методом ветвей и границ (бэктрекинг).
/// </summary>
public sealed class NQueensBenchmark : TimedAlgorithmBenchmark<int, QueensResult>, ITraceableBenchmark
{
    public const string AlgorithmId = "n-queens";
    public const int MaxSupportedN = 14;
    private const int MaxTraceNodes = 400;

    public NQueensBenchmark(NQueensAlgorithm algorithm)
        : base(algorithm, CreateMetadata(), n => n, result => result.Calls)
    {
    }

    /// <inheritdoc />
    public int TraceDefaultN => 4;

    /// <inheritdoc />
    public int TraceMaxN => 6;

    /// <inheritdoc />
    public RecursionTree BuildTrace(int n)
    {
        if (n < 1 || n > TraceMaxN)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, $"Для дерева n должно быть от 1 до {TraceMaxN}.");
        }

        return NQueensCounter.Trace(n, MaxTraceNodes);
    }

    private static AlgorithmMetadata CreateMetadata() => new(
        Id: AlgorithmId,
        Name: "N ферзей (поиск с возвратом)",
        Complexity: "O(n! / 2,54^n)",
        GrowthLabel: "n!/2,54^n",
        Description: "Поиск всех взаимно не атакующих расстановок N ферзей на доске N×N рекурсивным "
                     + "перебором с отсечением ветвей. Число рекурсивных вызовов растёт как O(n! / 2,54^n).",
        MinN: 1,
        MaxN: MaxSupportedN,
        Growth: n => PermutationGenerator.Factorial(n) / Math.Pow(2.54, n),
        EstimatedMsPerUnit: 5e-5);
}