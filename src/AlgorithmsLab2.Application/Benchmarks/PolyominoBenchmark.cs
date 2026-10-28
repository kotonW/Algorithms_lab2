using AlgorithmsLab2.Domain.Benchmarks;
using AlgorithmsLab2.Domain.Polyominoes;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>
/// Бенчмарк подсчёта полимино: время растёт как 4,06^n / n, максимум n = 17
/// (около 5·10^8 шагов рекурсии — десятки секунд на один прогон).
/// </summary>
public sealed class PolyominoBenchmark : TimedAlgorithmBenchmark<int, PolyominoResult>, ITraceableBenchmark
{
    /// <summary>Идентификатор алгоритма в настройках и реестре.</summary>
    public const string AlgorithmId = "polyominoes";

    /// <summary>Наибольшее n, допустимое в интерфейсе.</summary>
    public const int MaxSupportedN = 17;

    /// <summary>Основание роста: постоянная Клярнера, предел отношения соседних чисел полимино.</summary>
    public const double GrowthBase = 4.0626;

    /// <summary>Предел числа узлов дерева рекурсии.</summary>
    private const int MaxTraceNodes = 400;

    public PolyominoBenchmark(RedelmeierPolyominoAlgorithm algorithm)
        : base(algorithm, CreateMetadata(), n => n, result => result.Calls)
    {
    }

    /// <inheritdoc />
    public int TraceDefaultN => 3;

    /// <inheritdoc />
    /// <remarks>При n = 6 в дереве 308 узлов, при n = 7 уже 1068.</remarks>
    public int TraceMaxN => 6;

    /// <inheritdoc />
    public RecursionTree BuildTrace(int n)
    {
        if (n < 1 || n > TraceMaxN)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, $"Для дерева n должно быть от 1 до {TraceMaxN}.");
        }

        return PolyominoCounter.Trace(n, MaxTraceNodes);
    }

    private static AlgorithmMetadata CreateMetadata() => new(
        Id: AlgorithmId,
        Name: "Полимино (алгоритм Редельмайера)",
        Complexity: "O(4,06^n / n)",
        GrowthLabel: "4,06^n/n",
        Description: "Подсчёт фиксированных полимино — связных фигур из n клеток — рекурсией с возвратом. "
                     + "Каждая фигура строится ровно один раз; шагов примерно 0,41·4,06^n / n.",
        MinN: 1,
        MaxN: MaxSupportedN,
        Growth: n => Math.Pow(GrowthBase, n) / n,
        EstimatedMsPerUnit: 2e-5);
}
