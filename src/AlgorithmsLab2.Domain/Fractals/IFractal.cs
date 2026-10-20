using AlgorithmsLab2.Domain.Geometry;

namespace AlgorithmsLab2.Domain.Fractals;

/// <summary>
/// Фрактал, который умеет перечислять свои элементарные квадраты и
/// растеризовать себя рекурсивным алгоритмом.
/// </summary>
public interface IFractal
{
    FractalInfo Info { get; }

    /// <summary>Максимальная глубина, на которой фрактал ещё осмысленно рисовать.</summary>
    int MaxDepth { get; }

    /// <summary>Ленивое перечисление листовых квадратов заданной глубины.</summary>
    IEnumerable<FractalCell> Enumerate(int depth);

    /// <summary>Растеризация рекурсивным алгоритмом в изображение <paramref name="pixelSide"/> x <paramref name="pixelSide"/>.</summary>
    GrayRaster Rasterize(int depth, int pixelSide);
}
