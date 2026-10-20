using AlgorithmsLab2.Application.Painting;

namespace AlgorithmsLab2.Rendering;

/// <summary>Преобразование доменных цветов в цвета ScottPlot.</summary>
internal static class ColorExtensions
{
    public static ScottPlot.Color ToScottPlot(this Rgb rgb) => new(rgb.R, rgb.G, rgb.B);
}
