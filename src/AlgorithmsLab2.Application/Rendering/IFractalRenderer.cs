using AlgorithmsLab2.Domain.Fractals;

namespace AlgorithmsLab2.Application.Rendering;

/// <summary>Построение кадров фрактала. Чистая логика без привязки к UI и к ScottPlot.</summary>
public interface IFractalRenderer
{
    IFractal Fractal { get; }

    SierpinskiFrame Render(SierpinskiRenderRequest request, CancellationToken cancellationToken);
}
