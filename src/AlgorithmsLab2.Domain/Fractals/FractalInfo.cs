namespace AlgorithmsLab2.Domain.Fractals;

/// <summary>Паспорт фрактала: то, что показывается пользователю в панели информации.</summary>
public sealed record FractalInfo(string Name, string Description, double Dimension);
