namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Подбор коэффициента теоретической кривой c·f(n) методом наименьших квадратов.</summary>
public static class TheoryFit
{
    /// <summary>c = Σ(yᵢ·fᵢ) / Σ(fᵢ²), где yᵢ — среднее время, fᵢ = f(nᵢ). Для пустых данных — 0.</summary>
    public static double Coefficient(IReadOnlyList<BenchmarkPoint> points, Func<int, double> growth)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(growth);

        double numerator = 0;
        double denominator = 0;

        foreach (var point in points)
        {
            var f = growth(point.N);
            numerator += point.AverageMs * f;
            denominator += f * f;
        }

        return denominator > 0 ? numerator / denominator : 0;
    }
}
