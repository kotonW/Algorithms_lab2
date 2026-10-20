using AlgorithmsLab2.Application.Painting;
using AlgorithmsLab2.Domain.Geometry;

namespace AlgorithmsLab2.Rendering;

/// <summary>Рисует ковёр Серпинского в готовый <see cref="ScottPlot.Plot"/>.</summary>
public static class SierpinskiPlotRenderer
{
    /// <param name="aspectRatio">Отношение ширины области рисования к высоте: нужно, чтобы квадрат остался квадратом.</param>
    public static void Draw(ScottPlot.Plot plot, GrayRaster raster, FractalPalette palette, double aspectRatio = 1.0)
    {
        ArgumentNullException.ThrowIfNull(plot);
        ArgumentNullException.ThrowIfNull(raster);
        ArgumentNullException.ThrowIfNull(palette);

        plot.Clear();
        plot.HideGrid();
        plot.Axes.Frameless();
        plot.Axes.Margins(0, 0);
        plot.FigureBackground.Color = palette.Background.ToScottPlot();
        plot.DataBackground.Color = palette.Background.ToScottPlot();

        var image = RasterImageFactory.Create(raster, palette);
        var rect = new ScottPlot.CoordinateRect(0, 1, 0, 1);
        plot.Add.ImageRect(image, rect);

        // Единичный квадрат фрактала должен остаться квадратом: по более «длинной»
        // оси расширяем диапазон, чтобы масштаб по X и Y совпадал.
        var aspect = Math.Clamp(aspectRatio, 0.1, 10.0);
        var xRange = 1.0;
        var yRange = 1.0;

        if (aspect >= 1.0)
        {
            xRange = aspect;
        }
        else
        {
            yRange = 1.0 / aspect;
        }

        plot.Axes.SetLimits(
            0.5 - (xRange / 2.0),
            0.5 + (xRange / 2.0),
            0.5 - (yRange / 2.0),
            0.5 + (yRange / 2.0));
    }
}
