using System.Diagnostics;
using AlgorithmsLab2.Domain.Fractals;

namespace AlgorithmsLab2.Application.Rendering;

/// <summary>Растеризует ковёр Серпинского и собирает статистику кадра.</summary>
public sealed class FractalRenderer : IFractalRenderer
{
    public FractalRenderer(SierpinskiCarpet fractal) => Fractal = fractal;

    /// <inheritdoc />
    public IFractal Fractal { get; }

    /// <inheritdoc />
    public SierpinskiFrame Render(SierpinskiRenderRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var stopwatch = Stopwatch.StartNew();
        var raster = Fractal.Rasterize(request.Depth, request.PixelSide);
        stopwatch.Stop();

        cancellationToken.ThrowIfCancellationRequested();

        var statistics = new RenderStatistics(
            request.Depth,
            FractalMath.CellCount(request.Depth),
            FractalMath.Coverage(request.Depth),
            request.PixelSide,
            raster.FillRatio,
            stopwatch.Elapsed);

        return new SierpinskiFrame(raster, statistics);
    }
}
