using AlgorithmsLab2.Application.Apollonian;
using AlgorithmsLab2.Domain.Apollonian;

namespace AlgorithmsLab2.Rendering;

/// <summary>
/// Рисует аполлониеву решётку: внешняя окружность и круги, окрашенные по уровню рекурсии.
/// Каждый кадр строится с нуля. Контур кругов цвета фона, так что соседние круги разделены тонкой щелью.
/// </summary>
public static class ApollonianPlotRenderer
{
    /// <summary>Поля вокруг внешней окружности в долях её радиуса.</summary>
    private const double SceneMargin = 0.06;

    /// <param name="aspectRatio">Отношение ширины области рисования к высоте.</param>
    /// <param name="dark">Тёмная тема: от неё зависят фон и цвет внешней окружности.</param>
    public static void Draw(ScottPlot.Plot plot, ApollonianFrame frame, double aspectRatio, bool dark)
    {
        ArgumentNullException.ThrowIfNull(plot);
        ArgumentNullException.ThrowIfNull(frame);

        var colors = PlotColors.For(dark);

        plot.Clear();
        plot.HideGrid();
        plot.Axes.Frameless();
        plot.FigureBackground.Color = colors.Background;
        plot.DataBackground.Color = colors.Background;

        var outer = plot.Add.Circle(0, 0, ApolloniusGasket.OuterRadius);
        outer.FillColor = new ScottPlot.Color(0, 0, 0, 0);
        outer.LineColor = colors.Neutral;
        outer.LineWidth = 2f;

        // Цвет зависит от уровня относительно построенной глубины, поэтому при пошаговом показе не «плывёт».
        var colorSpan = Math.Max(frame.Gasket.Depth, 1);

        foreach (var circle in frame.Gasket.Circles)
        {
            if (circle.Level > frame.VisibleLevels)
            {
                continue;
            }

            var rgb = frame.Palette.ColorAt((double)circle.Level / colorSpan);

            var shape = plot.Add.Circle(circle.X, circle.Y, circle.Radius);
            shape.FillColor = new ScottPlot.Color(rgb.R, rgb.G, rgb.B);
            shape.LineColor = colors.Background;
            shape.LineWidth = circle.Level <= 3 ? 1.2f : 0.6f;
        }

        ApplyLimits(plot, aspectRatio);
    }

    private static void ApplyLimits(ScottPlot.Plot plot, double aspectRatio)
    {
        // Масштаб по осям одинаковый: иначе круги превратятся в эллипсы.
        // По более «длинной» оси добавляем поля, а не растягиваем сцену.
        var half = ApolloniusGasket.OuterRadius + SceneMargin;
        var aspect = Math.Clamp(aspectRatio, 0.1, 10.0);

        var xHalf = half;
        var yHalf = half;

        if (aspect >= 1.0)
        {
            xHalf = half * aspect;
        }
        else
        {
            yHalf = half / aspect;
        }

        plot.Axes.SetLimits(-xHalf, xHalf, -yHalf, yHalf);
    }
}
