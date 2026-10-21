using AlgorithmsLab2.Domain.Permutations;
using Xunit;

namespace AlgorithmsLab2.Tests;

public class PermutationGeneratorTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 6)]
    [InlineData(5, 120)]
    [InlineData(8, 40320)]
    public void Enumerate_ReturnsFactorial(int n, long expected)
    {
        Assert.Equal(expected, PermutationGenerator.Enumerate(n, null, TestContext.Current.CancellationToken));
        Assert.Equal(expected, PermutationGenerator.Factorial(n));
    }

    [Fact]
    public void GenerateAll_ProducesExactlyFactorialDistinctPermutations()
    {
        var all = PermutationGenerator.GenerateAll(5);

        Assert.Equal(120, all.Count);
        Assert.Equal(120, all.Select(p => string.Join(",", p)).Distinct().Count());
    }

    [Fact]
    public void GenerateAll_EachResultIsPermutationOfOneToN()
    {
        var expected = Enumerable.Range(1, 6).ToArray();

        foreach (var permutation in PermutationGenerator.GenerateAll(6))
        {
            Assert.Equal(expected, permutation.OrderBy(x => x).ToArray());
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(PermutationGenerator.MaxSupportedN + 1)]
    public void Enumerate_OutOfRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PermutationGenerator.Enumerate(n, null, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Enumerate_AlreadyCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => PermutationGenerator.Enumerate(8, null, cts.Token));
    }

    [Fact]
    public void Enumerate_CancelledDuringRun_Stops()
    {
        using var cts = new CancellationTokenSource();
        var visited = 0;

        Assert.Throws<OperationCanceledException>(() => PermutationGenerator.Enumerate(
            10,
            _ =>
            {
                if (++visited == 100)
                {
                    cts.Cancel();
                }
            },
            cts.Token));

        Assert.True(visited < 3_628_800);
    }
}
