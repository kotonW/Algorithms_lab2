namespace AlgorithmsLab2.Application.Rendering;

/// <summary>Что показать пользователю в панели информации после построения кадра.</summary>
public sealed record RenderStatistics(
    int Depth,
    long Cells,
    double Coverage,
    int PixelSide,
    double MeasuredFillRatio,
    TimeSpan RasterTime);
