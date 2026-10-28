namespace AlgorithmsLab2.Domain.Queens;

/// <summary>
/// Результат решения задачи о N ферзях: число найденных расстановок
/// и общее число рекурсивных вызовов (шагов перебора).
/// </summary>
/// <param name="Solutions">Количество найденных решений (расстановок N ферзей).</param>
/// <param name="Calls">Общее число посещённых узлов в дереве рекурсии.</param>
public readonly record struct QueensResult(long Solutions, long Calls);