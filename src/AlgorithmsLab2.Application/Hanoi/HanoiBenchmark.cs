using System.Diagnostics;
using AlgorithmsLab2.Domain.Hanoi;

namespace AlgorithmsLab2.Application.Hanoi;

/// <summary>Результат сравнения рекурсивного решения башен с решением через явный стек.</summary>
public sealed record HanoiBenchmarkResult(
    int DiskCount,
    int Moves,
    double RecursiveMicroseconds,
    double StackMicroseconds,
    bool SameMoves)
{
    /// <summary>Во сколько раз стек медленнее рекурсии (меньше единицы — быстрее).</summary>
    public double Ratio => RecursiveMicroseconds <= 0 ? 0 : StackMicroseconds / RecursiveMicroseconds;
}

/// <summary>
/// Сравнивает два способа получить последовательность ходов: рекурсию и явный стек.
/// Замеряется среднее время по нескольким прогонам, иначе на таких объёмах шумит таймер.
/// </summary>
public static class HanoiBenchmark
{
    private const int Iterations = 200;

    public static HanoiBenchmarkResult Run(int diskCount)
    {
        var disks = Math.Clamp(diskCount, 1, 12);

        var recursive = HanoiSolver.Solve(disks);
        var stack = HanoiSolver.SolveWithStack(disks);
        var sameMoves = recursive.Count == stack.Count && recursive.SequenceEqual(stack);

        // Прогрев: первый вызов включает JIT и портит замер.
        _ = HanoiSolver.Solve(disks);
        _ = HanoiSolver.SolveWithStack(disks);

        var recursiveTime = MeasureAverageMicroseconds(() => HanoiSolver.Solve(disks));
        var stackTime = MeasureAverageMicroseconds(() => HanoiSolver.SolveWithStack(disks));

        return new HanoiBenchmarkResult(disks, recursive.Count, recursiveTime, stackTime, sameMoves);
    }

    private static double MeasureAverageMicroseconds(Func<IReadOnlyList<HanoiMove>> solve)
    {
        var stopwatch = Stopwatch.StartNew();

        for (var iteration = 0; iteration < Iterations; iteration++)
        {
            _ = solve();
        }

        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMicroseconds / Iterations;
    }
}
