using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Domain.Benchmarks;
using AlgorithmsLab2.Domain.Queens;
using Xunit;

namespace AlgorithmsLab2.Tests;

public class NQueensTests
{
    // Известная последовательность OEIS A000170: число расстановок N ферзей для n = 1..10
    private static readonly long[] KnownSolutions = [1, 0, 0, 2, 10, 4, 40, 92, 352, 724];

    // Известное число вызовов рекурсии (узлов дерева перебора) OEIS A143248: n = 1..6
    private static readonly long[] KnownCalls = [2, 3, 6, 17, 54];

    private static NQueensBenchmark Create() => new(new NQueensAlgorithm());

    [Theory]
    [InlineData(1, 1)]
    [InlineData(4, 2)]
    [InlineData(5, 10)]
    [InlineData(6, 4)]
    [InlineData(7, 40)]
    [InlineData(8, 92)]
    public void Count_ReturnsKnownSolutions(int n, long expected)
    {
        var result = NQueensCounter.Count(n, TestContext.Current.CancellationToken);
        Assert.Equal(expected, result.Solutions);
    }

    [Fact]
    public void Count_MatchesKnownSequenceUpToTen()
    {
        for (var n = 1; n <= 10; n++)
        {
            var result = NQueensCounter.Count(n, TestContext.Current.CancellationToken);
            Assert.Equal(KnownSolutions[n - 1], result.Solutions);
        }
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 6)]
    [InlineData(4, 17)]
    [InlineData(5, 54)]
    public void Count_MatchesKnownCalls(int n, long expectedCalls)
    {
        var result = NQueensCounter.Count(n, TestContext.Current.CancellationToken);
        Assert.Equal(expectedCalls, result.Calls);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(NQueensCounter.MaxSupportedN + 1)]
    public void Count_OutOfRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NQueensCounter.Count(n, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Trace_ForFour_HasSeventeenNodes()
    {
        var tree = NQueensCounter.Trace(4);
        Assert.Equal(17, tree.NodeCount);
        Assert.False(tree.IsTruncated);
        Assert.Equal("∅", tree.Root.Label);
    }

    [Fact]
    public void Trace_IsTruncatedAtNodeLimit()
    {
        var tree = NQueensCounter.Trace(6, 50);
        Assert.True(tree.IsTruncated);
        Assert.Equal(50, tree.NodeCount);
    }

    [Fact]
    public void Benchmark_Metadata_And_Traceable()
    {
        var benchmark = Create();
        Assert.Equal("n-queens", benchmark.Metadata.Id);
        Assert.Equal(1, benchmark.Metadata.MinN);
        Assert.Equal(14, benchmark.Metadata.MaxN);

        var traceable = Assert.IsAssignableFrom<ITraceableBenchmark>(benchmark);
        Assert.Equal(4, traceable.TraceDefaultN);

        var trace = traceable.BuildTrace(4);
        Assert.Equal(17, trace.NodeCount);
    }
}