namespace AlgorithmsLab2.App.ViewModels;

/// <summary>Строка таблицы результатов: одна точка замера в виде готовых строк.</summary>
public sealed record BenchmarkRow(int N, string AverageText, string MinText, string MaxText);
