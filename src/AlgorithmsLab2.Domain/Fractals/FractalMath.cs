namespace AlgorithmsLab2.Domain.Fractals;

/// <summary>Аналитические характеристики ковра Серпинского — без перебора клеток.</summary>
public static class FractalMath
{
    /// <summary>Фрактальная размерность: log(8) / log(3) ≈ 1.8928.</summary>
    public static double Dimension => Math.Log(8) / Math.Log(3);

    /// <summary>Число листовых квадратов на глубине <paramref name="depth"/>: 8^depth.</summary>
    public static long CellCount(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(depth);
        return 1L << (3 * depth);
    }

    /// <summary>
    /// Доля площади единичного квадрата, занятая фракталом на глубине <paramref name="depth"/>: (8/9)^depth.
    /// Дополнение до единицы — площадь вырезанных «дыр».
    /// </summary>
    public static double Coverage(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(depth);
        return Math.Pow(8.0 / 9.0, depth);
    }

    /// <summary>Доля вырезанной площади: 1 − (8/9)^depth.</summary>
    public static double RemovedShare(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(depth);
        return 1.0 - Coverage(depth);
    }
}
