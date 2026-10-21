namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Грубая оценка длительности эксперимента — чтобы пользователь видел, что n = 15 это часы.</summary>
public static class BenchmarkEstimator
{
    // Не даём TimeSpan переполниться на огромных значениях.
    private const double MaxMilliseconds = 1e13;

    public static TimeSpan EstimateDuration(AlgorithmMetadata metadata, ExperimentConfig config)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        ArgumentNullException.ThrowIfNull(config);

        double milliseconds = 0;

        foreach (var n in config.Sizes(metadata))
        {
            milliseconds += metadata.Growth(n) * metadata.EstimatedMsPerUnit * config.RunsPerPoint;
        }

        return TimeSpan.FromMilliseconds(Math.Min(milliseconds, MaxMilliseconds));
    }
}
