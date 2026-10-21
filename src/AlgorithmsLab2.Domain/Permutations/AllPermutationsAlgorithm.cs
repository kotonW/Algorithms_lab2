using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Domain.Permutations;

/// <summary>
/// Перебор всех перестановок как замеряемый алгоритм: вход — n, результат — число перестановок (n!).
/// </summary>
public sealed class AllPermutationsAlgorithm : ICancellableTimedAlgorithm<int, long>
{
    /// <inheritdoc />
    public string Name => "Перебор всех перестановок";

    /// <inheritdoc />
    public long Execute(int input) => Execute(input, CancellationToken.None);

    /// <inheritdoc />
    public long Execute(int input, CancellationToken cancellationToken) =>
        PermutationGenerator.Enumerate(input, null, cancellationToken);
}
