using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Domain.Permutations;
using AlgorithmsLab2.Domain.Polyominoes;
using Xunit;

namespace AlgorithmsLab2.Tests;

public class PolyominoTests
{
    // Число фиксированных полимино из n клеток (OEIS A001168), n = 1..13.
    private static readonly long[] KnownCounts =
    [
        1, 2, 6, 19, 63, 216, 760, 2725, 9910, 36446, 135268, 505861, 1903890,
    ];

    private static PolyominoBenchmark Create() => new(new RedelmeierPolyominoAlgorithm());

    private static long CumulativeCalls(int n) => KnownCounts.Take(n).Sum();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 6)]
    [InlineData(4, 19)]
    [InlineData(5, 63)]
    [InlineData(8, 2725)]
    [InlineData(10, 36446)]
    public void CountFixed_ReturnsKnownNumberOfPolyominoes(int n, long expected)
    {
        var result = PolyominoCounter.CountFixed(n, TestContext.Current.CancellationToken);

        Assert.Equal(expected, result.Count);
    }

    [Fact]
    public void CountFixed_MatchesKnownSequenceUpToTwelve()
    {
        for (var n = 1; n <= 12; n++)
        {
            var result = PolyominoCounter.CountFixed(n, TestContext.Current.CancellationToken);

            Assert.Equal(KnownCounts[n - 1], result.Count);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(9)]
    public void CountFixed_CallsEqualSumOfCountsOfAllSizes(int n)
    {
        var result = PolyominoCounter.CountFixed(n, TestContext.Current.CancellationToken);

        Assert.Equal(CumulativeCalls(n), result.Calls);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(PolyominoCounter.MaxSupportedN + 1)]
    public void CountFixed_OutOfRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => PolyominoCounter.CountFixed(n, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void CountFixed_AlreadyCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => PolyominoCounter.CountFixed(10, cts.Token));
    }

    [Fact]
    public void CountFixed_CancelledDuringRun_Stops()
    {
        // n = 22 — это десятки миллиардов шагов: отмена через 20 мс успевает сработать.
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(20));

        Assert.Throws<OperationCanceledException>(() => PolyominoCounter.CountFixed(22, cts.Token));
    }

    [Fact]
    public void Benchmark_Metadata_DescribesAlgorithm()
    {
        var metadata = Create().Metadata;

        Assert.Equal("polyominoes", metadata.Id);
        Assert.Equal(1, metadata.MinN);
        Assert.Equal(17, metadata.MaxN);
        Assert.Equal(PolyominoBenchmark.GrowthBase, metadata.Growth(1), 9);
        Assert.Equal(Math.Pow(PolyominoBenchmark.GrowthBase, 5) / 5, metadata.Growth(5), 9);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(4, 28)]
    [InlineData(6, 307)]
    public void Benchmark_RunOnce_ReturnsNumberOfSteps(int n, long expectedCalls)
    {
        Assert.Equal(expectedCalls, Create().RunOnce(n, TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(18)]
    public void Benchmark_RunOnce_OutOfRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Create().RunOnce(n, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void TheoryCurve_FollowsNumberOfStepsFromFourthPoint()
    {
        var benchmark = Create();
        var points = new List<BenchmarkPoint>();

        for (var n = 1; n <= 12; n++)
        {
            var steps = benchmark.RunOnce(n, TestContext.Current.CancellationToken);
            points.Add(new BenchmarkPoint(n, steps, steps, steps, steps));
        }

        var coefficient = TheoryFit.Coefficient(points, benchmark.Metadata.Growth);

        foreach (var point in points.Where(p => p.N >= 4))
        {
            var predicted = coefficient * benchmark.Metadata.Growth(point.N);
            var relativeError = Math.Abs(predicted - point.Operations) / point.Operations;

            Assert.True(relativeError < 0.02, $"n = {point.N}: ошибка {relativeError:P2}");
        }
    }

    [Fact]
    public void Registry_HoldsBothAlgorithmsInRegistrationOrder()
    {
        var permutations = new PermutationsBenchmark(new AllPermutationsAlgorithm());
        var registry = new BenchmarkRegistry([permutations, Create()]);

        Assert.Equal(2, registry.Algorithms.Count);
        Assert.Same(permutations, registry.Default);
        Assert.Same(registry.Algorithms[1], registry.Find("polyominoes"));
    }

    [Fact]
    public void Runner_MeasuresPolyominoesAndOperationsMatchSteps()
    {
        var series = new BenchmarkRunner().Run(
            Create(),
            new ExperimentConfig(9, 2),
            null,
            TestContext.Current.CancellationToken);

        Assert.Equal(9, series.Points.Count);
        Assert.Equal(CumulativeCalls(9), series.Points[^1].Operations);
    }
}
