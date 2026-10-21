namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Одна точка графика: результат нескольких прогонов на заданном n.</summary>
/// <param name="N">Размер входа.</param>
/// <param name="AverageMs">Среднее время прогона, мс.</param>
/// <param name="MinMs">Минимальное время прогона, мс.</param>
/// <param name="MaxMs">Максимальное время прогона, мс.</param>
/// <param name="Operations">Число операций одного прогона (для перестановок — n!).</param>
public sealed record BenchmarkPoint(int N, double AverageMs, double MinMs, double MaxMs, long Operations);
