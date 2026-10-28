using AlgorithmsLab2.Domain.Apollonian;

namespace AlgorithmsLab2.Application.Apollonian;

/// <summary>Числа для панели информации: сколько кругов видно, какую долю внешнего круга они покрывают.</summary>
/// <param name="VisibleLevels">До какого уровня включительно показана решётка.</param>
/// <param name="CircleCount">Число показанных кругов (без внешней окружности).</param>
/// <param name="Coverage">Доля площади внешнего круга, занятая кругами решётки: Σr² / R².</param>
/// <param name="SmallestRadius">Радиус самого мелкого из показанных кругов.</param>
public sealed record GasketStatistics(int VisibleLevels, long CircleCount, double Coverage, double SmallestRadius)
{
    /// <summary>Доля площади внешнего круга, оставшаяся не закрытой. С ростом глубины стремится к нулю.</summary>
    public double Uncovered => 1.0 - Coverage;

    /// <summary>Считает статистику по кругам уровней 0..<paramref name="visibleLevels"/>.</summary>
    public static GasketStatistics For(GasketResult gasket, int visibleLevels)
    {
        ArgumentNullException.ThrowIfNull(gasket);

        var visible = Math.Clamp(visibleLevels, 0, gasket.Depth);

        long count = 0;
        var area = 0.0;
        var smallest = double.MaxValue;

        foreach (var circle in gasket.Circles)
        {
            if (circle.Level > visible)
            {
                continue;
            }

            count++;
            area += circle.Radius * circle.Radius;
            smallest = Math.Min(smallest, circle.Radius);
        }

        var coverage = area / (ApolloniusGasket.OuterRadius * ApolloniusGasket.OuterRadius);

        return new GasketStatistics(visible, count, coverage, count == 0 ? 0.0 : smallest);
    }
}
