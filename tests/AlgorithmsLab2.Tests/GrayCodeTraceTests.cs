using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Domain.Benchmarks;
using AlgorithmsLab2.Domain.GrayCode;
using Xunit;

namespace AlgorithmsLab2.Tests;

public class GrayCodeTraceTests
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
    public void Trace_ForThree_HasFifteenNodesInExecutionOrder()
    {
        var tree = GrayCodeCounter.Trace(3);
        var nodes = Flatten(tree.Root);

        Assert.Equal(15, tree.NodeCount);
        Assert.Equal(15, nodes.Count);
        Assert.False(tree.IsTruncated);
        Assert.Equal(Enumerable.Range(0, 15), nodes.Select(node => node.Order));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(6)]
    public void Trace_NodeCountEqualsNumberOfRecursiveCalls(int n)
    {
        var calls = GrayCodeCounter.Count(n, TestContext.Current.CancellationToken).Calls;

        Assert.Equal(calls, GrayCodeCounter.Trace(n).NodeCount);
    }

    [Fact]
    public void Trace_TreeIsStrictlyBinary()
    {
        var nodes = Flatten(GrayCodeCounter.Trace(5).Root);

        Assert.All(nodes, node => Assert.True(node.Children.Count is 0 or 2));
        Assert.Equal(32, nodes.Count(node => node.Children.Count == 0));
    }

    [Fact]
    public void Trace_LeavesAreGrayCodesInGenerationOrder()
    {
        var leaves = Flatten(GrayCodeCounter.Trace(3).Root)
            .Where(node => node.Children.Count == 0)
            .Select(node => node.Label);

        Assert.Equal(["000", "001", "011", "010", "110", "111", "101", "100"], leaves);

        var generated = GrayCodeCounter.Generate(3, TestContext.Current.CancellationToken)
            .Select(code => Convert.ToString(code, 2).PadLeft(3, '0'));

        Assert.Equal(generated, leaves);
    }

    [Fact]
    public void Trace_NeighbouringLeavesDifferInExactlyOneBit()
    {
        var codes = Flatten(GrayCodeCounter.Trace(6).Root)
            .Where(node => node.Children.Count == 0)
            .Select(node => Convert.ToInt32(node.Label, 2))
            .ToList();

        for (var i = 1; i < codes.Count; i++)
        {
            Assert.Equal(1, int.PopCount(codes[i - 1] ^ codes[i]));
        }
    }

    [Fact]
    public void Trace_RootAndFirstLevelAreLabelledWithPrefixAndDirection()
    {
        var root = GrayCodeCounter.Trace(3).Root;

        Assert.Equal("∅", root.Label);
        Assert.Equal(["0", "1←"], root.Children.Select(child => child.Label));
    }

    [Fact]
    public void Trace_IsTruncatedAtNodeLimit()
    {
        var tree = GrayCodeCounter.Trace(8, 100);

        Assert.True(tree.IsTruncated);
        Assert.Equal(100, tree.NodeCount);
        Assert.Equal(100, Flatten(tree.Root).Count);
    }

    [Fact]
    public void Trace_InvalidArguments_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GrayCodeCounter.Trace(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => GrayCodeCounter.Trace(GrayCodeCounter.MaxSupportedN + 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => GrayCodeCounter.Trace(3, 0));
    }

    [Fact]
    public void Benchmark_IsTraceableAndFitsIntoNodeLimit()
    {
        var benchmark = new GrayCodeBenchmark(new GrayCodeAlgorithm());
        var traceable = Assert.IsAssignableFrom<ITraceableBenchmark>(benchmark);

        Assert.Equal(3, traceable.TraceDefaultN);
        Assert.Equal(15, traceable.BuildTrace(traceable.TraceDefaultN).NodeCount);

        var largest = traceable.BuildTrace(traceable.TraceMaxN);
        Assert.Equal(255, largest.NodeCount);
        Assert.False(largest.IsTruncated);
    }

    [Fact]
    public void Benchmark_BuildTrace_OutOfRange_Throws()
    {
        ITraceableBenchmark traceable = new GrayCodeBenchmark(new GrayCodeAlgorithm());

        Assert.Throws<ArgumentOutOfRangeException>(() => traceable.BuildTrace(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => traceable.BuildTrace(8));
    }
}
