namespace AlgorithmsLab2.Domain.Catalan;

/// <summary>Результат наивного вычисления числа Каталана: само число и сколько рекурсивных вызовов понадобилось.</summary>
/// <param name="Value">Число Каталана C(n).</param>
/// <param name="Calls">Число рекурсивных вызовов; для наивной рекурсии оно равно ровно 3^n.</param>
public readonly record struct CatalanResult(long Value, long Calls);
