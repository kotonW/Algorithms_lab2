using AlgorithmsLab2.Application.Apollonian;
using AlgorithmsLab2.Application.Canvases;
using ScottPlot.Avalonia;

namespace AlgorithmsLab2.Rendering;

/// <summary>Мост между ScottPlot и ViewModel для аполлониевой решётки. Вызывать только из UI-потока.</summary>
public sealed class AvaPlotApollonianCanvas : IApollonianCanvas
{
    private readonly AvaPlot _avaPlot;
    private readonly Func<bool> _isDark;

    /// <param name="isDark">Текущая тема: график подстраивает цвета под интерфейс.</param>
    public AvaPlotApollonianCanvas(AvaPlot avaPlot, Func<bool>? isDark = null)
    {
        _avaPlot = avaPlot ?? throw new ArgumentNullException(nameof(avaPlot));
        _isDark = isDark ?? (() => true);

        // Красим фон сразу, чтобы на тёмной теме не было вспышки белого до первого кадра.
        var colors = PlotColors.For(_isDark());
        _avaPlot.Plot.FigureBackground.Color = colors.Background;
        _avaPlot.Plot.DataBackground.Color = colors.Background;
    }

    /// <inheritdoc />
    public void Show(ApollonianFrame frame)
    {
        ApollonianPlotRenderer.Draw(_avaPlot.Plot, frame, AspectRatio(), _isDark());
        _avaPlot.Refresh();
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
}
