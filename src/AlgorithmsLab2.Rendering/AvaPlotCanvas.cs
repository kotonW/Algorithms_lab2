using System.Globalization;
using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Application.Canvases;
using AlgorithmsLab2.Application.Painting;
using AlgorithmsLab2.Domain.Geometry;
using AlgorithmsLab2.Domain.Hanoi;
using ScottPlot.Avalonia;

namespace AlgorithmsLab2.Rendering;

/// <summary>
/// Мост между ScottPlot и ViewModel: рисует в <see cref="AvaPlot"/> и обновляет картинку.
/// Все методы должны вызываться из UI-потока — Plot не потокобезопасен.
/// </summary>
public sealed class AvaPlotCanvas : IFractalCanvas, IHanoiCanvas, IBenchmarkCanvas
{
    private static readonly ScottPlot.Color HighlightColor = new(255, 214, 102);
    private static readonly ScottPlot.Color LabelTextColor = new(20, 22, 27);

    private readonly AvaPlot _avaPlot;
    private readonly Func<bool> _isDark;

    private BenchmarkSeries? _series;
    private bool _showTheory = true;
    private ScottPlot.IPlottable? _pointMarker;
    private ScottPlot.IPlottable? _pointLabel;

    /// <param name="isDark">Текущая тема: графики подстраивают цвета под интерфейс.</param>
    public AvaPlotCanvas(AvaPlot avaPlot, Func<bool>? isDark = null)
    {
        _avaPlot = avaPlot ?? throw new ArgumentNullException(nameof(avaPlot));
        _isDark = isDark ?? (() => true);

        ApplyBaselineTheme();
    }

    /// <inheritdoc />
    public void Show(GrayRaster raster, FractalPalette palette)
    {
        SierpinskiPlotRenderer.Draw(_avaPlot.Plot, raster, palette, AspectRatio());
        _avaPlot.Refresh();
    }

    /// <inheritdoc />
    void IHanoiCanvas.Show(HanoiSnapshot snapshot)
    {
        HanoiPlotRenderer.Draw(_avaPlot.Plot, snapshot, AspectRatio(), _isDark());
        _avaPlot.Refresh();
    }

    /// <inheritdoc />
    void IBenchmarkCanvas.Show(BenchmarkSeries series, bool showTheory)
    {
        _series = series;
        _showTheory = showTheory;
        _pointMarker = null;
        _pointLabel = null;

        BenchmarkPlotRenderer.Draw(_avaPlot.Plot, series, showTheory, _isDark());
        _avaPlot.Refresh();
    }

    /// <inheritdoc />
    void IBenchmarkCanvas.RefreshTheme() => RefreshTheme();

    /// <inheritdoc />
    /// <remarks>
    /// Координаты приходят в DIP (как их отдаёт Avalonia), а рендер ScottPlot живёт в пикселях.
    /// Масштаб считаем по фактическому размеру отрисованной области, а не по DPI:
    /// так выбор точки работает и при другом масштабировании экрана.
    /// </remarks>
    bool IBenchmarkCanvas.TryPickPoint(float x, float y, out BenchmarkPoint? point)
    {
        point = null;

        if (_series is null)
        {
            return false;
        }

        var plot = _avaPlot.Plot;
        var bounds = _avaPlot.Bounds;
        var figure = plot.LastRender.FigureRect;

        var scaleX = bounds.Width > 0 && figure.Width > 0 ? figure.Width / (float)bounds.Width : 1f;
        var scaleY = bounds.Height > 0 && figure.Height > 0 ? figure.Height / (float)bounds.Height : 1f;
        var targetX = x * scaleX;
        var targetY = y * scaleY;

        var bestDistance = double.MaxValue;

        foreach (var candidate in _series.Points)
        {
            var pixel = plot.GetPixel(new ScottPlot.Coordinates(candidate.N, candidate.AverageMs));
            var dx = pixel.X - targetX;
            var dy = pixel.Y - targetY;
            var distance = Math.Sqrt((dx * dx) + (dy * dy));

            if (distance < bestDistance)
            {
                bestDistance = distance;
                point = candidate;
            }
        }

        // Промах мимо точки: подпись не показываем.
        if (point is null || bestDistance > 30)
        {
            point = null;
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    void IBenchmarkCanvas.ShowPointInfo(BenchmarkPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);
        HidePointInfo();

        var plot = _avaPlot.Plot;
        var yRange = plot.Axes.Left.Max - plot.Axes.Left.Min;
        var labelY = point.AverageMs + (yRange * 0.06);

        _pointMarker = plot.Add.Marker(
            point.N,
            point.AverageMs,
            ScottPlot.MarkerShape.FilledCircle,
            15,
            HighlightColor);

        var text = string.Create(
            CultureInfo.CurrentCulture,
            $"n = {point.N}\n{point.AverageMs:F3} мс");

        _pointLabel = plot.Add.Text(text, point.N, labelY);

        if (_pointLabel is ScottPlot.Plottables.Text label)
        {
            label.LabelFontSize = 13f;
            label.LabelBold = true;
            label.LabelFontColor = HighlightColor;
            label.LabelBackgroundColor = LabelTextColor;
            label.LabelBorderColor = HighlightColor;
            label.LabelBorderWidth = 1;
            label.LabelPadding = 6;
        }

        _avaPlot.Refresh();
    }

    /// <inheritdoc />
    void IBenchmarkCanvas.HidePointInfo() => HidePointInfo();

    /// <summary>
    /// Перекраска под текущую тему: фон, оси и, если данные уже есть, сам график.
    /// </summary>
    private void RefreshTheme()
    {
        ApplyBaselineTheme();

        if (_series is not null)
        {
            _pointMarker = null;
            _pointLabel = null;
            BenchmarkPlotRenderer.Draw(_avaPlot.Plot, _series, _showTheory, _isDark());
        }

        _avaPlot.Refresh();
    }

    /// <summary>
    /// До первого кадра ScottPlot рисует белую плоскость. Красим фон сразу при подключении,
    /// чтобы на тёмной теме не было вспышки белого.
    /// </summary>
    private void ApplyBaselineTheme()
    {
        var colors = PlotColors.For(_isDark());
        var plot = _avaPlot.Plot;

        plot.FigureBackground.Color = colors.Background;
        plot.DataBackground.Color = colors.Background;
        plot.Axes.Color(colors.Text);
    }

    /// <summary>
    /// Пропорции области рисования. До первой раскладки контрол имеет нулевой размер,
    /// поэтому подставляем разумное значение по умолчанию.
    /// </summary>
    private double AspectRatio()
    {
        var bounds = _avaPlot.Bounds;

        if (bounds.Width < 1 || bounds.Height < 1)
        {
            return 1.5;
        }

        return bounds.Width / bounds.Height;
    }

    private void HidePointInfo()
    {
        var plot = _avaPlot.Plot;
        var removed = false;

        if (_pointMarker is not null)
        {
            plot.Remove(_pointMarker);
            _pointMarker = null;
            removed = true;
        }

        if (_pointLabel is not null)
        {
            plot.Remove(_pointLabel);
            _pointLabel = null;
            removed = true;
        }

        if (removed)
        {
            _avaPlot.Refresh();
        }
    }
}
