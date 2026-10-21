using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Domain.Permutations;
using Xunit;

namespace AlgorithmsLab2.Tests;

public class BenchmarkRunnerTests
{
    private static readonly AlgorithmMetadata FakeMetadata = new(
        "fake", "Фейк", "O(n)", "n", "Тестовый алгоритм", 1, 20, n => n, 1e-3);

    private sealed class FakeBenchmark(Action<int>? onRun = null) : IAlgorithmBenchmark
    {
        public int Calls { get; private set; }

        public Dictionary<int, int> CallsByN { get; } = [];

        public AlgorithmMetadata Metadata => FakeMetadata;

        public long RunOnce(int n, CancellationToken cancellationToken)
        {
            Calls++;
            CallsByN[n] = CallsByN.GetValueOrDefault(n) + 1;
            onRun?.Invoke(Calls);
            return n;
        }
    }

    private sealed class ImmediateProgress<T> : IProgress<T>
    {
        public List<T> Items { get; } = [];

        public void Report(T value) => Items.Add(value);
    }

    [Fact]
    public void Run_ProducesOnePointPerN()
    {
        var series = new BenchmarkRunner().Run(new FakeBenchmark(), new ExperimentConfig(6, 2), null, CancellationToken.None);

        Assert.Equal(Enumerable.Range(1, 6), series.Points.Select(p => p.N));
    }

    [Fact]
    public void Run_MakesConfiguredNumberOfRunsPerPoint()
    {
        var benchmark = new FakeBenchmark();

        new BenchmarkRunner().Run(benchmark, new ExperimentConfig(5, 4), null, CancellationToken.None);

        Assert.Equal(20, benchmark.Calls);
        Assert.All(benchmark.CallsByN.Values, calls => Assert.Equal(4, calls));
    }

    [Fact]
    public void Run_StepSkipsSizes()
    {
        var series = new BenchmarkRunner().Run(new FakeBenchmark(), new ExperimentConfig(9, 1, 4), null, CancellationToken.None);

        Assert.Equal([1, 5, 9], series.Points.Select(p => p.N));
    }

    [Fact]
    public void Run_ReportsProgressForEveryPoint()
    {
        var progress = new ImmediateProgress<BenchmarkProgress>();

        new BenchmarkRunner().Run(new FakeBenchmark(), new ExperimentConfig(4, 1), progress, CancellationToken.None);

        Assert.Equal(4, progress.Items.Count);
        Assert.Equal(1.0, progress.Items[^1].Fraction);
    }

    [Fact]
    public void Run_AverageLiesBetweenMinAndMax()
    {
        var series = new BenchmarkRunner().Run(
            new PermutationsBenchmark(new AllPermutationsAlgorithm()),
            new ExperimentConfig(8, 3),
            null,
            CancellationToken.None);

        Assert.All(series.Points, point =>
        {
            Assert.True(point.MinMs <= point.AverageMs);
            Assert.True(point.AverageMs <= point.MaxMs);
        });
        Assert.Equal(40320, series.Points[^1].Operations);
    }

    [Fact]
    public void Run_CancellationStopsMeasurement()
    {
        using var cts = new CancellationTokenSource();
        var benchmark = new FakeBenchmark(calls =>
        {
            if (calls == 3)
            {
                cts.Cancel();
            }
        });

        Assert.Throws<OperationCanceledException>(
            () => new BenchmarkRunner().Run(benchmark, new ExperimentConfig(10, 1), null, cts.Token));

        Assert.True(benchmark.Calls < 10);
    }

    [Theory]
    [InlineData(0, 3)]
    [InlineData(21, 3)]
    [InlineData(5, 0)]
    [InlineData(5, 101)]
    public void Run_InvalidConfig_Throws(int maxN, int runs)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BenchmarkRunner().Run(new FakeBenchmark(), new ExperimentConfig(maxN, runs), null, CancellationToken.None));
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsConfig()
    {
        var ok = ExperimentConfig.TryParse("10", "5", FakeMetadata, out var config, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal(new ExperimentConfig(10, 5), config);
    }

    [Theory]
    [InlineData("abc", "3")]
    [InlineData("10", "x")]
    [InlineData("99", "3")]
    [InlineData("10", "0")]
    [InlineData(null, "3")]
    public void TryParse_InvalidInput_ReturnsError(string? maxN, string? runs)
    {
        var ok = ExperimentConfig.TryParse(maxN, runs, FakeMetadata, out var config, out var error);

        Assert.False(ok);
        Assert.Null(config);
        Assert.False(string.IsNullOrEmpty(error));
    }

    [Fact]
    public void TheoryFit_ExactLinearData_RecoversCoefficient()
    {
        var points = Enumerable.Range(1, 5)
            .Select(n => new BenchmarkPoint(n, 2.5 * n, 2.5 * n, 2.5 * n, n))
            .ToList();

        Assert.Equal(2.5, TheoryFit.Coefficient(points, n => n), 9);
    }

    [Fact]
    public void Estimator_GrowsFasterThanLinearly_ForFactorial()
    {
        var metadata = new PermutationsBenchmark(new AllPermutationsAlgorithm()).Metadata;

        var small = BenchmarkEstimator.EstimateDuration(metadata, new ExperimentConfig(10, 1));
        var large = BenchmarkEstimator.EstimateDuration(metadata, new ExperimentConfig(15, 1));

        Assert.True(large > small * 1000);
    }
}
