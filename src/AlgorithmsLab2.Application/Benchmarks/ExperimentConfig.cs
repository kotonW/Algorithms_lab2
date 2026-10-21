using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Параметры эксперимента: до какого n мерить и сколько независимых прогонов делать на точку.</summary>
/// <param name="MaxN">Наибольшее n.</param>
/// <param name="RunsPerPoint">Число независимых прогонов на каждую точку.</param>
/// <param name="Step">Шаг по n (по умолчанию 1).</param>
public sealed record ExperimentConfig(int MaxN, int RunsPerPoint, int Step = 1)
{
    /// <summary>Верхняя граница числа прогонов на точку.</summary>
    public const int MaxRuns = 100;

    /// <summary>Значения n, на которых будут сделаны замеры: от минимума алгоритма до <see cref="MaxN"/>.</summary>
    public IReadOnlyList<int> Sizes(AlgorithmMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        Validate(metadata);

        var sizes = new List<int>();

        for (var n = metadata.MinN; n <= MaxN; n += Step)
        {
            sizes.Add(n);
        }

        return sizes;
    }

    /// <summary>Проверяет параметры относительно ограничений алгоритма.</summary>
    public void Validate(AlgorithmMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        if (MaxN < metadata.MinN || MaxN > metadata.MaxN)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxN),
                MaxN,
                $"Максимальное n должно быть от {metadata.MinN} до {metadata.MaxN}.");
        }

        if (RunsPerPoint is < 1 or > MaxRuns)
        {
            throw new ArgumentOutOfRangeException(
                nameof(RunsPerPoint),
                RunsPerPoint,
                $"Число прогонов должно быть от 1 до {MaxRuns}.");
        }

        if (Step < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(Step), Step, "Шаг должен быть не меньше 1.");
        }
    }

    /// <summary>Разбирает значения из текстовых полей. Возвращает <c>false</c> и текст ошибки, если они некорректны.</summary>
    public static bool TryParse(
        string? maxN,
        string? runs,
        AlgorithmMetadata metadata,
        [NotNullWhen(true)] out ExperimentConfig? config,
        [NotNullWhen(false)] out string? error)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        config = null;

        if (!int.TryParse(maxN, NumberStyles.Integer, CultureInfo.CurrentCulture, out var parsedMaxN))
        {
            error = "Максимальное n должно быть целым числом.";
            return false;
        }

        if (!int.TryParse(runs, NumberStyles.Integer, CultureInfo.CurrentCulture, out var parsedRuns))
        {
            error = "Число прогонов должно быть целым числом.";
            return false;
        }

        var candidate = new ExperimentConfig(parsedMaxN, parsedRuns);

        try
        {
            candidate.Validate(metadata);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            error = exception.Message;
            return false;
        }

        config = candidate;
        error = null;
        return true;
    }
}
