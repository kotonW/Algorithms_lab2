namespace AlgorithmsLab2.Domain.Geometry;

/// <summary>
/// Монохромное растровое изображение фрактала: 1 байт на пиксель,
/// 0 — пусто (дыра), 255 — закрашено.
/// </summary>
public sealed class GrayRaster
{
    private readonly byte[] _pixels;

    public GrayRaster(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        Width = width;
        Height = height;
        _pixels = new byte[width * height];
    }

    public int Width { get; }

    public int Height { get; }

    /// <summary>Пиксели построчно, сверху вниз.</summary>
    public ReadOnlySpan<byte> Pixels => _pixels;

    /// <summary>Количество закрашенных пикселей.</summary>
    public int FilledCount { get; private set; }

    /// <summary>Доля закрашенных пикселей — эмпирическая площадь фрактала.</summary>
    public double FillRatio => (double)FilledCount / _pixels.Length;

    /// <summary>Закрашивает прямоугольник, обрезая его по границам изображения.</summary>
    internal void FillRect(int left, int top, int width, int height)
    {
        var x0 = Math.Max(0, left);
        var y0 = Math.Max(0, top);
        var x1 = Math.Min(Width, left + width);
        var y1 = Math.Min(Height, top + height);

        if (x0 >= x1 || y0 >= y1)
        {
            return;
        }

        for (var y = y0; y < y1; y++)
        {
            var rowStart = y * Width;
            for (var x = x0; x < x1; x++)
            {
                if (_pixels[rowStart + x] == 0)
                {
                    _pixels[rowStart + x] = 255;
                    FilledCount++;
                }
            }
        }
    }
}
