namespace AlgorithmsLab2.Domain.Geometry;

/// <summary>
/// Прямоугольник в единичной системе координат фрактала: X и Y растут вправо и вверх.
/// </summary>
public readonly record struct RectD(double X, double Y, double Width, double Height)
{
    public double Right => X + Width;

    public double Top => Y + Height;
}
