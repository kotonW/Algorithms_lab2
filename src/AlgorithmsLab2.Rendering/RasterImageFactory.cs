using AlgorithmsLab2.Application.Painting;
using AlgorithmsLab2.Domain.Geometry;

namespace AlgorithmsLab2.Rendering;

/// <summary>Собирает изображение ScottPlot из монохромного растра и палитры.</summary>
internal static class RasterImageFactory
{
    public static ScottPlot.Image Create(GrayRaster raster, FractalPalette palette)
    {
        ArgumentNullException.ThrowIfNull(raster);
        ArgumentNullException.ThrowIfNull(palette);

        var pixels = new byte[raster.Height, raster.Width, 3];
        var background = palette.Background;
        var foreground = palette.Foreground;
        var source = raster.Pixels;

        for (var y = 0; y < raster.Height; y++)
        {
            var rowOffset = y * raster.Width;

            for (var x = 0; x < raster.Width; x++)
            {
                var color = source[rowOffset + x] != 0 ? foreground : background;
                pixels[y, x, 0] = color.R;
                pixels[y, x, 1] = color.G;
                pixels[y, x, 2] = color.B;
            }
        }

        return new ScottPlot.Image(pixels);
    }
}
