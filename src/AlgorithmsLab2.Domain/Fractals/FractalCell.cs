using AlgorithmsLab2.Domain.Geometry;

namespace AlgorithmsLab2.Domain.Fractals;

/// <summary>Один квадрат, полученный на уровне <paramref name="Level"/> рекурсии.</summary>
public readonly record struct FractalCell(RectD Rect, int Level);
