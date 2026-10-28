using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Domain.GrayCode;

/// <summary>
/// Генерация кода Грея как замеряемый алгоритм: вход — n, результат — <see cref="GrayCodeResult"/>.
/// </summary>
public sealed class GrayCodeAlgorithm : ICancellableTimedAlgorithm<int, GrayCodeResult>
{
    /// <inheritdoc />
    public string Name => "Генерация кода Грея (рекурсивная)";

    /// <inheritdoc />
    public GrayCodeResult Execute(int input) =>
        GrayCodeCounter.Count(input, CancellationToken.None);

    /// <inheritdoc />
    public GrayCodeResult Execute(int input, CancellationToken cancellationToken) =>
        GrayCodeCounter.Count(input, cancellationToken);
}