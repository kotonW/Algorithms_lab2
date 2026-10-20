namespace AlgorithmsLab2.Domain.Hanoi;

/// <summary>
/// Два способа решить головоломку: рекурсией и явным стеком.
/// Сравнение времени их работы показывает плату за рекурсивные вызовы.
/// </summary>
public static class HanoiSolver
{
    public static IReadOnlyList<HanoiMove> Solve(int diskCount, int fromPeg = 0, int toPeg = 2, int auxiliaryPeg = 1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(diskCount, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(diskCount, 20);

        var moves = new List<HanoiMove>(MinimalMoves(diskCount));
        Move(diskCount, fromPeg, toPeg, auxiliaryPeg, moves);
        return moves;
    }

    /// <summary>
    /// То же решение без рекурсии: состояние обхода хранится в явном стеке.
    /// Результат обязан совпадать с <see cref="Solve"/> ход в ход.
    /// </summary>
    public static IReadOnlyList<HanoiMove> SolveWithStack(int diskCount, int fromPeg = 0, int toPeg = 2, int auxiliaryPeg = 1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(diskCount, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(diskCount, 20);

        var moves = new List<HanoiMove>(MinimalMoves(diskCount));

        // Задача либо «перенести count колец», либо «сделать сам ход» (IsMove).
        var stack = new Stack<(int Count, int From, int To, int Auxiliary, bool IsMove)>();
        stack.Push((diskCount, fromPeg, toPeg, auxiliaryPeg, false));

        while (stack.Count > 0)
        {
            var (count, from, to, auxiliary, isMove) = stack.Pop();

            if (isMove)
            {
                moves.Add(new HanoiMove(count, from, to));
                continue;
            }

            if (count == 0)
            {
                continue;
            }

            // Порядок выполнения снизу вверх: перенос n−1 на промежуточный стержень,
            // затем сам ход, затем перенос n−1 с промежуточного на целевой.
            stack.Push((count - 1, auxiliary, to, from, false));
            stack.Push((count, from, to, auxiliary, true));
            stack.Push((count - 1, from, auxiliary, to, false));
        }

        return moves;
    }

    /// <summary>Минимальное число ходов: 2^n − 1.</summary>
    public static int MinimalMoves(int diskCount) => (1 << diskCount) - 1;

    private static void Move(int count, int from, int to, int auxiliary, List<HanoiMove> sink)
    {
        if (count == 0)
        {
            return;
        }

        Move(count - 1, from, auxiliary, to, sink);
        sink.Add(new HanoiMove(count, from, to));
        Move(count - 1, auxiliary, to, from, sink);
    }
}
