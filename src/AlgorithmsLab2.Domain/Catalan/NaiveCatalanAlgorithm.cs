using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Domain.Catalan;

/// <summary>Числа Каталана наивной рекурсией как замеряемый алгоритм: вход — n, результат — C(n) и число вызовов.</summary>
public sealed class NaiveCatalanAlgorithm : ICancellableTimedAlgorithm<int, CatalanResult>
{
    /// <inheritdoc />
    public string Name => "Числа Каталана (наивная рекурсия)";

    /// <inheritdoc />
    public CatalanResult Execute(int input) => CatalanNumbers.ComputeRecursive(input, CancellationToken.None);

    /// <inheritdoc />
    public CatalanResult Execute(int input, CancellationToken cancellationToken) =>
        CatalanNumbers.ComputeRecursive(input, cancellationToken);
}
