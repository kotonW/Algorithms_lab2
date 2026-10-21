using System.Diagnostics;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>
/// Методика первой лабораторной: для каждого n делается несколько независимых прогонов,
/// время каждого замеряется <see cref="Stopwatch"/>, в точку попадают среднее, минимум и максимум.
/// </summary>
public sealed class BenchmarkRunner
{
    /// <summary>Выполняет эксперимент синхронно. Для UI вызывать из фонового потока.</summary>
    /// <exception cref="OperationCanceledException">Если замер отменён через <paramref name="cancellationToken"/>.</exception>
    public BenchmarkSeries Run(
        IAlgorithmBenchmark benchmark,
        ExperimentConfig config,
        IProgress<BenchmarkProgress>? progress,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(benchmark);
        ArgumentNullException.ThrowIfNull(config);

        var sizes = config.Sizes(benchmark.Metadata);
        var points = new List<BenchmarkPoint>(sizes.Count);

        foreach (var n in sizes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var times = new double[config.RunsPerPoint];
            long operations = 0;

            for (var run = 0; run < times.Length; run++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var stopwatch = Stopwatch.StartNew();
                operations = benchmark.RunOnce(n, cancellationToken);
                stopwatch.Stop();

                times[run] = stopwatch.Elapsed.TotalMilliseconds;
            }

            var point = new BenchmarkPoint(n, times.Average(), times.Min(), times.Max(), operations);
            points.Add(point);
            progress?.Report(new BenchmarkProgress(points.Count, sizes.Count, point));
        }

        return new BenchmarkSeries(benchmark.Metadata, points);
    }
}
