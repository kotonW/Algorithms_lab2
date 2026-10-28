namespace AlgorithmsLab2.Domain.Apollonian;

/// <summary>
/// Построенная решётка: все круги до указанной глубины. Внешняя единичная окружность в список не входит —
/// она есть всегда и в построении не участвует как «заполняющий» круг.
/// </summary>
/// <param name="Depth">Глубина рекурсии, до которой построены круги.</param>
/// <param name="Circles">Круги уровней 0..<paramref name="Depth"/>.</param>
public sealed record GasketResult(int Depth, IReadOnlyList<GasketCircle> Circles);
