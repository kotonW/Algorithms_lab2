namespace AlgorithmsLab2.Domain.Polyominoes;

/// <summary>Результат подсчёта полимино: сколько фигур найдено и сколько шагов рекурсии потребовалось.</summary>
/// <param name="Count">Число фиксированных полимино из n клеток.</param>
/// <param name="Calls">
/// Число шагов рекурсии (размещений клетки). Это сумма чисел полимино всех размеров от 1 до n,
/// поэтому шагов примерно в 1,33 раза больше, чем найденных фигур.
/// </param>
public readonly record struct PolyominoResult(long Count, long Calls);
