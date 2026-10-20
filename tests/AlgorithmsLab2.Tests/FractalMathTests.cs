using AlgorithmsLab2.Domain.Fractals;
using Xunit;

namespace AlgorithmsLab2.Tests;

public sealed class FractalMathTests
{
    [Theory]
    [InlineData(0, 1L)]
    [InlineData(1, 8L)]
    [InlineData(2, 64L)]
    [InlineData(3, 512L)]
    [InlineData(6, 262_144L)]
    [InlineData(8, 16_777_216L)]
    public void CellCount_ReturnsPowerOfEight(int depth, long expected) =>
        Assert.Equal(expected, FractalMath.CellCount(depth));

    [Fact]
    public void Coverage_StartsFromWholeSquare() =>
        Assert.Equal(1.0, FractalMath.Coverage(0), 12);

    [Fact]
    public void Coverage_AfterFirstLevel_IsEightNinths() =>
        Assert.Equal(8.0 / 9.0, FractalMath.Coverage(1), 12);

    [Fact]
    public void Coverage_DecreasesWithDepth()
    {
        for (var depth = 1; depth <= 8; depth++)
        {
            Assert.True(FractalMath.Coverage(depth) < FractalMath.Coverage(depth - 1));
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(8)]
    public void RemovedShare_ComplementsCoverage(int depth) =>
        Assert.Equal(1.0, FractalMath.Coverage(depth) + FractalMath.RemovedShare(depth), 12);

    [Fact]
    public void Dimension_IsLogEightBaseThree() =>
        Assert.Equal(Math.Log(8) / Math.Log(3), FractalMath.Dimension, 12);

    [Fact]
    public void CellCount_RejectsNegativeDepth() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => FractalMath.CellCount(-1));
}
