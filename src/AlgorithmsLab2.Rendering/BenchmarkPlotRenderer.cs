using System.Globalization;
using AlgorithmsLab2.Application.Benchmarks;

namespace AlgorithmsLab2.Rendering;

/// <summary>
/// График замеров: измеренное среднее время и теоретическая кривая c·f(n) оранжевым пунктиром.
/// Коэффициент c подобран методом наименьших квадратов и показан в легенде.
/// </summary>
public static class BenchmarkPlotRenderer
{
    public static void Draw(ScottPlot.Plot plot, BenchmarkSeries series, bool showTheory, bool dark)
    {
        ArgumentNullException.ThrowIfNull(plot);
        ArgumentNullException.ThrowIfNull(series);

        var colors = PlotColors.For(dark);

        plot.Clear();

        plot.FigureBackground.Color = colors.Background;
        plot.DataBackground.Color = colors.Background;
        plot.Axes.Color(colors.Text);
        plot.Grid.MajorLineColor = colors.Grid;

        plot.XLabel("n");
        plot.YLabel("Время, мс");

        var points = series.Points;

        if (points.Count == 0)
        {
            plot.Axes.SetLimits(0, series.Metadata.MaxN, 0, 1);
            return;
        }

        var xs = points.Select(point => (double)point.N).ToArray();
        var ys = points.Select(point => point.AverageMs).ToArray();

        var measured = plot.Add.Scatter(xs, ys);
        measured.Color = colors.Measured;
        measured.LineWidth = 2;
        measured.MarkerSize = 8;
        measured.LegendText = "Измерения (среднее)";

        if (showTheory)
        {
            var growth = series.Metadata.Growth;
            var coefficient = TheoryFit.Coefficient(points, growth);

            if (coefficient > 0)
            {
                var theoryYs = points.Select(point => coefficient * growth(point.N)).ToArray();

                var theory = plot.Add.Scatter(xs, theoryYs);
                theory.Color = colors.Theory;
                theory.LineWidth = 2;
                theory.MarkerSize = 0;
                theory.LinePattern = ScottPlot.LinePattern.Dashed;
                theory.LegendText = string.Create(
                    CultureInfo.CurrentCulture,
                    $"c·{series.Metadata.GrowthLabel}, c = {coefficient:E2} мс");
            }
        }

        plot.ShowLegend(ScottPlot.Alignment.UpperLeft);
        plot.Legend.BackgroundColor = colors.LegendBackground;
        plot.Legend.FontColor = colors.Text;
        plot.Legend.OutlineColor = colors.LegendBorder;

        plot.Axes.AutoScale();
    }
}
