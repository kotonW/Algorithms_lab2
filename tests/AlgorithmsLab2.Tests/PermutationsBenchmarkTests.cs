using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Domain.Permutations;
using Xunit;

namespace AlgorithmsLab2.Tests;

public class PermutationsBenchmarkTests
{
    private static PermutationsBenchmark Create() => new(new AllPermutationsAlgorithm());

    [Fact]
    public void Metadata_DescribesFactorialAlgorithm()
    {
        var metadata = Create().Metadata;

        Assert.Equal("permutations", metadata.Id);
        Assert.Equal("O(n!)", metadata.Complexity);
        Assert.Equal(1, metadata.MinN);
        Assert.Equal(15, metadata.MaxN);
        Assert.Equal(120, metadata.Growth(5));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(4, 24)]
    [InlineData(7, 5040)]
    public void RunOnce_ReturnsFactorial(int n, long expected)
    {
        Assert.Equal(expected, Create().RunOnce(n, CancellationToken.None));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    public void RunOnce_OutOfRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Create().RunOnce(n, CancellationToken.None));
    }

    [Fact]
    public void Registry_FindsAlgorithmById()
    {
        var registry = new BenchmarkRegistry([Create()]);

        Assert.Single(registry.Algorithms);
        Assert.Same(registry.Default, registry.Find("permutations"));
        Assert.Null(registry.Find("unknown"));
    }

    [Fact]
    public void Registry_DuplicateIds_Throw()
    {
        Assert.Throws<ArgumentException>(() => new BenchmarkRegistry([Create(), Create()]));
    }

    [Fact]
    public void Registry_Empty_Throws()
    {
        Assert.Throws<ArgumentException>(() => new BenchmarkRegistry([]));
    }
}
