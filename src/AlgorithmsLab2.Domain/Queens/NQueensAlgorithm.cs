using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Domain.Queens;

/// <summary>
/// Алгоритм расстановки N ферзей как замеряемая задача.
/// </summary>
public sealed class NQueensAlgorithm : ICancellableTimedAlgorithm<int, QueensResult>
{
    public string Name => "Расстановка N ферзей (поиск с возвратом)";

    public QueensResult Execute(int input) =>
        NQueensCounter.Count(input, CancellationToken.None);

    public QueensResult Execute(int input, CancellationToken cancellationToken) =>
        NQueensCounter.Count(input, cancellationToken);
}