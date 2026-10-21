namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Общая основа бенчмарков: хранит описание и проверяет диапазон n.</summary>
public abstract class AlgorithmBenchmark : IAlgorithmBenchmark
{
    protected AlgorithmBenchmark(AlgorithmMetadata metadata)
    {
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    /// <inheritdoc />
    public AlgorithmMetadata Metadata { get; }

    /// <inheritdoc />
    public abstract long RunOnce(int n, CancellationToken cancellationToken);

    /// <summary>Бросает исключение, если n вне допустимого для алгоритма диапазона.</summary>
    protected void ValidateSize(int n)
    {
        if (n < Metadata.MinN || n > Metadata.MaxN)
        {
            throw new ArgumentOutOfRangeException(
                nameof(n),
                n,
                $"Для «{Metadata.Name}» n должно быть от {Metadata.MinN} до {Metadata.MaxN}.");
        }
    }
}
