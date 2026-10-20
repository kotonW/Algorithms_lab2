using AlgorithmsLab2.Domain.Geometry;

namespace AlgorithmsLab2.Application.Rendering;

/// <summary>Готовый кадр: растр и его статистика.</summary>
public sealed record SierpinskiFrame(GrayRaster Raster, RenderStatistics Statistics);
