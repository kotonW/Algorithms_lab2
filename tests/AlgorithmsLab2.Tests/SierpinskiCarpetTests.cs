using AlgorithmsLab2.Domain.Fractals;
using AlgorithmsLab2.Domain.Geometry;
using Xunit;

namespace AlgorithmsLab2.Tests;

public sealed class SierpinskiCarpetTests
{
    private readonly SierpinskiCarpet _carpet = new();

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 8)]
    [InlineData(2, 64)]
    [InlineData(3, 512)]
    [InlineData(4, 4_096)]
    public void Enumerate_ReturnsEightToTheDepthCells(int depth, int expected) =>
        Assert.Equal(expected, _carpet.Enumerate(depth).Count());

    [Fact]
    public void Enumerate_AllCellsStayInsideUnitSquare()
    {
        foreach (var cell in _carpet.Enumerate(4))
        {
            Assert.InRange(cell.Rect.X, 0, 1);
            Assert.InRange(cell.Rect.Y, 0, 1);
            Assert.InRange(cell.Rect.Right, 0, 1.000_000_1);
            Assert.InRange(cell.Rect.Top, 0, 1.000_000_1);
        }
    }

    [Fact]
    public void Enumerate_CellsOfOneLevelDoNotOverlap()
    {
        var cells = _carpet.Enumerate(3).Select(cell => cell.Rect).ToArray();

        for (var i = 0; i < cells.Length; i++)
        {
            for (var j = i + 1; j < cells.Length; j++)
            {
                Assert.False(Overlaps(cells[i], cells[j]), $"Клетки {i} и {j} пересекаются.");
            }
        }
    }

    [Fact]
    public void Enumerate_DefaultsToWholeSquareAtLevelZero()
    {
        var cell = Assert.Single(_carpet.Enumerate(0));

        Assert.Equal(new RectD(0, 0, 1, 1), cell.Rect);
        Assert.Equal(0, cell.Level);
    }

    [Fact]
    public void Rasterize_AtDepthZero_FillsWholeSquare()
    {
        var raster = _carpet.Rasterize(0, 27);

        Assert.Equal(1.0, raster.FillRatio, 12);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    public void Rasterize_MatchesTheoreticalCoverage(int depth)
    {
        // Сторону берём такую, чтобы на каждую клетку приходился хотя бы пиксель.
        var side = (int)Math.Pow(3, depth);
        var raster = _carpet.Rasterize(depth, side);

        Assert.Equal(FractalMath.Coverage(depth), raster.FillRatio, 2);
    }

    [Fact]
    public void Rasterize_ReturnsRasterOfTheRequestedSize()
    {
        var raster = _carpet.Rasterize(3, 128);

        Assert.Equal(128, raster.Width);
        Assert.Equal(128, raster.Height);
        Assert.Equal(128 * 128, raster.Pixels.Length);
    }

    [Fact]
    public void Rasterize_IsDeterministic()
    {
        var first = _carpet.Rasterize(4, 243).Pixels.ToArray();
        var second = _carpet.Rasterize(4, 243).Pixels.ToArray();

        Assert.Equal(first, second);
    }

    [Fact]
    public void Rasterize_RejectsNegativeDepth() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => _carpet.Rasterize(-1, 100));

    [Fact]
    public void Rasterize_RejectsEmptySize() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => _carpet.Rasterize(2, 0));

    private static bool Overlaps(RectD left, RectD right) =>
        left.X < right.Right - 1e-9
        && right.X < left.Right - 1e-9
        && left.Y < right.Top - 1e-9
        && right.Y < left.Top - 1e-9;
}
