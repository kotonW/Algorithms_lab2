using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Domain.Benchmarks;
using AlgorithmsLab2.Domain.Permutations;
using AlgorithmsLab2.Domain.Polyominoes;
using Xunit;

namespace AlgorithmsLab2.Tests;

public class RecursionTreeTests
{
    private static List<RecursionNode> Flatten(RecursionNode root)
    {
        var result = new List<RecursionNode>();

        void Walk(RecursionNode node)
        {
            result.Add(node);

            foreach (var child in node.Children)
            {
                Walk(child);
            }
        }

        Walk(root);
        return result;
    }

    [Fact]
    public void PermutationTrace_ForThree_HasSixteenNodesInExecutionOrder()
    {
        var tree = PermutationGenerator.Trace(3);
        var nodes = Flatten(tree.Root);

        Assert.Equal(16, tree.NodeCount);
        Assert.Equal(16, nodes.Count);
        Assert.False(tree.IsTruncated);
        Assert.Equal(Enumerable.Range(0, 16), nodes.Select(node => node.Order));
    }

    [Fact]
    public void PermutationTrace_LeavesAreRealPermutationsInGenerationOrder()
    {
        var tree = PermutationGenerator.Trace(3);

        var leaves = Flatten(tree.Root)
            .Where(node => node.Children.Count == 0)
            .Select(node => node.Label);

        Assert.Equal(["123", "132", "213", "231", "321", "312"], leaves);
    }

    [Fact]
    public void PermutationTrace_RootHasNChildrenAndLeavesCountIsFactorial()
    {
        var tree = PermutationGenerator.Trace(4);

        Assert.Equal("∅", tree.Root.Label);
        Assert.Equal(4, tree.Root.Children.Count);
        Assert.Equal(24, Flatten(tree.Root).Count(node => node.Children.Count == 0));
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 5)]
    [InlineData(4, 65)]
    [InlineData(5, 326)]
    public void PermutationTrace_NodeCountMatchesNumberOfCalls(int n, int expectedNodes)
    {
        Assert.Equal(expectedNodes, PermutationGenerator.Trace(n).NodeCount);
    }

    [Fact]
    public void PermutationTrace_IsTruncatedAtNodeLimit()
    {
        var tree = PermutationGenerator.Trace(6, 50);

        Assert.True(tree.IsTruncated);
        Assert.Equal(50, tree.NodeCount);
        Assert.Equal(50, Flatten(tree.Root).Count);
    }

    [Fact]
    public void PermutationTrace_InvalidArguments_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PermutationGenerator.Trace(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => PermutationGenerator.Trace(3, 0));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    public void PolyominoTrace_NodeCountIsCallsPlusSyntheticRoot(int n)
    {
        var calls = PolyominoCounter.CountFixed(n, TestContext.Current.CancellationToken).Calls;
        var tree = PolyominoCounter.Trace(n);

        Assert.Equal(calls + 1, tree.NodeCount);
        Assert.Equal(tree.NodeCount, Flatten(tree.Root).Count);
        Assert.False(tree.IsTruncated);
    }

    [Fact]
    public void PolyominoTrace_StartsWithOriginAndGrowsToNeighbours()
    {
        var tree = PolyominoCounter.Trace(2);

        Assert.Equal("∅", tree.Root.Label);

        var origin = Assert.Single(tree.Root.Children);
        Assert.Equal("(0,0)", origin.Label);
        Assert.Equal(
            new HashSet<string> { "(0,1)", "(1,0)" },
            origin.Children.Select(child => child.Label).ToHashSet());
    }

    [Fact]
    public void PolyominoTrace_OrdersAreSequentialPreorder()
    {
        var tree = PolyominoCounter.Trace(4);
        var nodes = Flatten(tree.Root);

        Assert.Equal(Enumerable.Range(0, nodes.Count), nodes.Select(node => node.Order));
    }

    [Fact]
    public void PolyominoTrace_IsTruncatedAtNodeLimit()
    {
        var tree = PolyominoCounter.Trace(6, 100);

        Assert.True(tree.IsTruncated);
        Assert.Equal(100, tree.NodeCount);
        Assert.Equal(100, Flatten(tree.Root).Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(PolyominoCounter.MaxSupportedN + 1)]
    public void PolyominoTrace_OutOfRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PolyominoCounter.Trace(n));
    }

    [Fact]
    public void PermutationsBenchmark_IsTraceableAndFitsIntoNodeLimit()
    {
        var benchmark = new PermutationsBenchmark(new AllPermutationsAlgorithm());
        var traceable = Assert.IsAssignableFrom<ITraceableBenchmark>(benchmark);

        Assert.Equal(3, traceable.TraceDefaultN);
        Assert.Equal(16, traceable.BuildTrace(traceable.TraceDefaultN).NodeCount);

        var largest = traceable.BuildTrace(traceable.TraceMaxN);
        Assert.False(largest.IsTruncated);
    }

    [Fact]
    public void PolyominoBenchmark_IsTraceableAndFitsIntoNodeLimit()
    {
        var benchmark = new PolyominoBenchmark(new RedelmeierPolyominoAlgorithm());
        var traceable = Assert.IsAssignableFrom<ITraceableBenchmark>(benchmark);

        Assert.Equal(3, traceable.TraceDefaultN);
        Assert.Equal(10, traceable.BuildTrace(traceable.TraceDefaultN).NodeCount);

        var largest = traceable.BuildTrace(traceable.TraceMaxN);
        Assert.Equal(308, largest.NodeCount);
        Assert.False(largest.IsTruncated);
    }

    [Fact]
    public void BuildTrace_OutOfRange_Throws()
    {
        ITraceableBenchmark permutations = new PermutationsBenchmark(new AllPermutationsAlgorithm());
        ITraceableBenchmark polyominoes = new PolyominoBenchmark(new RedelmeierPolyominoAlgorithm());

        Assert.Throws<ArgumentOutOfRangeException>(() => permutations.BuildTrace(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => permutations.BuildTrace(6));
        Assert.Throws<ArgumentOutOfRangeException>(() => polyominoes.BuildTrace(7));
    }
}
