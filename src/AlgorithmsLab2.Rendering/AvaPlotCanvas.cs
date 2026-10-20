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
public sealed class AvaPlotCanvas : IFractalCanvas, IHanoiCanvas
{
    private readonly AvaPlot _avaPlot;

    public AvaPlotCanvas(AvaPlot avaPlot) => _avaPlot = avaPlot ?? throw new ArgumentNullException(nameof(avaPlot));

    /// <inheritdoc />
    public void Show(GrayRaster raster, FractalPalette palette)
    {
        SierpinskiPlotRenderer.Draw(_avaPlot.Plot, raster, palette, AspectRatio());
        _avaPlot.Refresh();
    }

    /// <inheritdoc />
    void IHanoiCanvas.Show(HanoiSnapshot snapshot)
    {
        HanoiPlotRenderer.Draw(_avaPlot.Plot, snapshot, AspectRatio());
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
