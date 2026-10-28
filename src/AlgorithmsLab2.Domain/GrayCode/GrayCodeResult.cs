namespace AlgorithmsLab2.Domain.GrayCode;

/// <summary>
/// Результат генерации кода Грея: сколько кодов получено, сколько шагов рекурсии
/// и XOR-контрольная сумма всех кодов (для проверки корректности без хранения
/// всей последовательности в памяти).
/// </summary>
/// <param name="Count">Число сгенерированных кодов = 2^n.</param>
/// <param name="Calls">
/// Число рекурсивных вызовов: 2^(n+1) − 1 (все внутренние узлы дерева рекурсии + листья).
/// Ровно в 2 раза больше <see cref="Count"/> минус 1, но полезно как мера «работы рекурсии».
/// </param>
/// <param name="Checksum">XOR всех сгенерированных кодов длины n.</param>
public readonly record struct GrayCodeResult(long Count, long Calls, long Checksum);