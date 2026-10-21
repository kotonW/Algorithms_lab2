namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Прогресс замера: сколько точек готово и какая появилась последней.</summary>
public sealed record BenchmarkProgress(int CompletedPoints, int TotalPoints, BenchmarkPoint Point)
{
    /// <summary>Доля выполненной работы по числу точек, 0..1.</summary>
    public double Fraction => TotalPoints == 0 ? 1 : (double)CompletedPoints / TotalPoints;
}
