namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Описание замеряемого алгоритма: имя, сложность, допустимый диапазон n и теоретическая кривая.</summary>
/// <param name="Id">Стабильный идентификатор (хранится в настройках).</param>
/// <param name="Name">Название для выпадающего списка.</param>
/// <param name="Complexity">Асимптотика, например «O(n!)».</param>
/// <param name="GrowthLabel">Запись функции роста для легенды, например «n!».</param>
/// <param name="Description">Краткое описание.</param>
/// <param name="MinN">Наименьшее допустимое n.</param>
/// <param name="MaxN">Наибольшее допустимое n.</param>
/// <param name="Growth">Функция роста f(n); теоретическое время — это c·f(n).</param>
/// <param name="EstimatedMsPerUnit">Грубая оценка миллисекунд на единицу f(n): нужна только для прогноза длительности.</param>
public sealed record AlgorithmMetadata(
    string Id,
    string Name,
    string Complexity,
    string GrowthLabel,
    string Description,
    int MinN,
    int MaxN,
    Func<int, double> Growth,
    double EstimatedMsPerUnit);
