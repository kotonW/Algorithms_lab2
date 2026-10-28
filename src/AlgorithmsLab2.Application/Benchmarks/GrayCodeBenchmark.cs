using AlgorithmsLab2.Domain.GrayCode;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>
/// Бенчмарк генерации кода Грея: время растёт как 2^n.
/// Верхняя граница n = 24 выбрана так, чтобы один прогон укладывался в ~секунды;
/// при необходимости можно поднять до 26, если железо быстрое.
/// </summary>
public sealed class GrayCodeBenchmark : TimedAlgorithmBenchmark<int, GrayCodeResult>
{
    /// <summary>Идентификатор алгоритма в настройках и реестре.</summary>
    public const string AlgorithmId = "gray-code";

    /// <summary>Наибольшее n, допустимое в интерфейсе.</summary>
    public const int MaxSupportedN = 24;

    /// <summary>Основание роста: каждый новый бит удваивает число кодов.</summary>
    public const double GrowthBase = 2.0;

    public GrayCodeBenchmark(GrayCodeAlgorithm algorithm)
        : base(algorithm, CreateMetadata(), n => n, result => result.Calls)
    {
    }

    private static AlgorithmMetadata CreateMetadata() => new(
        Id: AlgorithmId,
        Name: "Код Грея (рекурсивная генерация)",
        Complexity: "O(2^n)",
        GrowthLabel: "2^n",
        Description: "Рекурсивная генерация всех 2^n кодов Грея длины n по правилу "
                     + "G(n) = 0·G(n−1) ++ 1·reverse(G(n−1)). Соседние коды различаются одним битом. "
                     + "Число шагов рекурсии 2^(n+1) − 1, память O(n).",
        MinN: 4,
        MaxN: MaxSupportedN,
        Growth: n => Math.Pow(GrowthBase, n),
        EstimatedMsPerUnit: 1e-5);
}