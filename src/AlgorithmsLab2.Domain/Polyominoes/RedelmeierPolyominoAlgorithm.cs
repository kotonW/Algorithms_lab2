using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Domain.Polyominoes;

/// <summary>Подсчёт полимино как замеряемый алгоритм: вход — n, результат — число фигур и число шагов рекурсии.</summary>
public sealed class RedelmeierPolyominoAlgorithm : ICancellableTimedAlgorithm<int, PolyominoResult>
{
    /// <inheritdoc />
    public string Name => "Подсчёт полимино (алгоритм Редельмайера)";

    /// <inheritdoc />
    public PolyominoResult Execute(int input) => PolyominoCounter.CountFixed(input, CancellationToken.None);

    /// <inheritdoc />
    public PolyominoResult Execute(int input, CancellationToken cancellationToken) =>
        PolyominoCounter.CountFixed(input, cancellationToken);
}
