using AlgorithmsLab2.Application.Painting;
using AlgorithmsLab2.Domain.Geometry;

namespace AlgorithmsLab2.Application.Canvases;

/// <summary>
/// Куда рисовать ковёр Серпинского. Реализация живёт в слое визуализации,
/// поэтому ViewModel не знает про ScottPlot.
/// </summary>
public interface IFractalCanvas
{
    void Show(GrayRaster raster, FractalPalette palette);
}
