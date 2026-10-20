using AlgorithmsLab2.Application.Rendering;
using Xunit;

namespace AlgorithmsLab2.Tests;

public sealed class RenderLimitsTests
{
    [Theory]
    [InlineData(0, 243)]
    [InlineData(3, 243)]
    [InlineData(5, 243)]
    [InlineData(6, 729)]
    [InlineData(7, 2187)]
    [InlineData(8, 2187)]
    public void PixelSideFor_StaysWithinBounds(int depth, int expected) =>
        Assert.Equal(expected, RenderLimits.PixelSideFor(depth));

    [Theory]
    [InlineData(-5, 0)]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    [InlineData(99, 7)]
    public void ClampDepth_KeepsDepthInRange(int depth, int expected) =>
        Assert.Equal(expected, RenderLimits.ClampDepth(depth));

    [Fact]
    public void ForDepth_UsesClampedDepth()
    {
        var request = SierpinskiRenderRequest.ForDepth(42);

        Assert.Equal(RenderLimits.MaxDepth, request.Depth);
        Assert.Equal(RenderLimits.MaxPixelSide, request.PixelSide);
    }

    [Fact]
    public void Renderer_ProducesStatisticsConsistentWithRaster()
    {
        var renderer = new FractalRenderer(new Domain.Fractals.SierpinskiCarpet());
        var frame = renderer.Render(SierpinskiRenderRequest.ForDepth(4), CancellationToken.None);

        Assert.Equal(4, frame.Statistics.Depth);
        Assert.Equal(4_096, frame.Statistics.Cells);
        Assert.Equal(frame.Raster.FillRatio, frame.Statistics.MeasuredFillRatio, 12);
        Assert.InRange(frame.Statistics.Coverage, frame.Raster.FillRatio - 0.01, frame.Raster.FillRatio + 0.01);
    }
}
