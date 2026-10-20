using AlgorithmsLab2.Domain.Fractals;
using AlgorithmsLab2.Domain.Geometry;

namespace AlgorithmsLab2.Domain.Fractals;

/// <summary>
/// Ковёр Серпинского: квадрат делится на 9 частей, центральная вырезается,
/// с каждой из 8 оставшихся повторяется то же самое.
/// </summary>
public sealed class SierpinskiCarpet : IFractal
{
    /// <inheritdoc />
    public FractalInfo Info { get; } = new(
        "Ковёр Серпинского",
        "Квадрат делится на 9 равных частей, центральная удаляется, а с восемью оставшимися "
        + "повторяется то же самое. На каждом уровне вырезается одна девятая часть площади.",
        FractalMath.Dimension);

    /// <inheritdoc />
    public int MaxDepth => 7;

    /// <inheritdoc />
    public IEnumerable<FractalCell> Enumerate(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(depth);
        return Walk(new RectD(0, 0, 1, 1), 0, depth);
    }

    /// <inheritdoc />
    public GrayRaster Rasterize(int depth, int pixelSide)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(depth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pixelSide);

        var raster = new GrayRaster(pixelSide, pixelSide);
        Fill(raster, 0, 0, pixelSide, depth);
        return raster;
    }

    /// <summary>Рекурсивный обход: метод вызывает сам себя для восьми подквадратов.</summary>
    private static IEnumerable<FractalCell> Walk(RectD rect, int level, int depth)
    {
        if (level == depth)
        {
            yield return new FractalCell(rect, level);
            yield break;
        }

        var size = rect.Width / 3.0;

        for (var row = 0; row < 3; row++)
        {
            for (var column = 0; column < 3; column++)
            {
                if (row == 1 && column == 1)
                {
                    continue;
                }

                var child = new RectD(rect.X + (column * size), rect.Y + (row * size), size, size);

                foreach (var cell in Walk(child, level + 1, depth))
                {
                    yield return cell;
                }
            }
        }
    }

    /// <summary>
    /// Рекурсивное заполнение: пока уровень не дошёл до нуля, квадрат дробится на 8 частей;
    /// когда часть стала размером с пиксель, она закрашивается целиком.
    /// </summary>
    private static void Fill(GrayRaster raster, int left, int top, int size, int level)
    {
        if (level <= 0 || size <= 1)
        {
            raster.FillRect(left, top, size, size);
            return;
        }

        for (var row = 0; row < 3; row++)
        {
            for (var column = 0; column < 3; column++)
            {
                // Центральная девятая часть — «дыра» фрактала.
                if (row == 1 && column == 1)
                {
                    continue;
                }

                var x1 = left + (int)Math.Round(column * size / 3.0);
                var x2 = left + (int)Math.Round((column + 1) * size / 3.0);
                var y1 = top + (int)Math.Round(row * size / 3.0);
                var y2 = top + (int)Math.Round((row + 1) * size / 3.0);

                Fill(raster, x1, y1, Math.Min(x2 - x1, y2 - y1), level - 1);
            }
        }
    }
}
