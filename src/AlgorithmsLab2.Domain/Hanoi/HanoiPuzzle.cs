namespace AlgorithmsLab2.Domain.Hanoi;

/// <summary>
/// Состояние головоломки и проигрывание заранее построенного списка ходов.
/// Позволяет идти вперёд, назад и прыгать на любой ход.
/// </summary>
public sealed class HanoiPuzzle
{
    private readonly List<int>[] _pegs = [[], [], []];

    public HanoiPuzzle(int diskCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(diskCount, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(diskCount, 20);

        DiskCount = diskCount;
        Moves = HanoiSolver.Solve(diskCount);
        Reset();
    }

    public int DiskCount { get; }

    public IReadOnlyList<HanoiMove> Moves { get; }

    public int TotalMoves => Moves.Count;

    public int MovesMade { get; private set; }

    public bool IsComplete => MovesMade >= TotalMoves;

    public HanoiSnapshot Snapshot() => new(
        _pegs.Select(peg => (IReadOnlyList<int>)peg.ToArray()).ToArray(),
        DiskCount,
        MovesMade,
        TotalMoves,
        MovesMade > 0 ? Moves[MovesMade - 1] : null);

    /// <summary>Сбрасывает головоломку: все кольца на первом стержне, большее снизу.</summary>
    public HanoiSnapshot Reset()
    {
        foreach (var peg in _pegs)
        {
            peg.Clear();
        }

        for (var disk = DiskCount; disk >= 1; disk--)
        {
            _pegs[0].Add(disk);
        }

        MovesMade = 0;
        return Snapshot();
    }

    /// <summary>Делает один ход вперёд. В конце списка ничего не меняет.</summary>
    public HanoiSnapshot Step()
    {
        if (IsComplete)
        {
            return Snapshot();
        }

        var move = Moves[MovesMade];
        _pegs[move.FromPeg].RemoveAt(_pegs[move.FromPeg].Count - 1);
        _pegs[move.ToPeg].Add(move.Disk);
        MovesMade++;

        return Snapshot();
    }

    /// <summary>
    /// Возвращает состояние после <paramref name="moveIndex"/> ходов (пересчётом с начала).
    /// Индекс за пределами диапазона прижимается к нему: откатывать «до начала» — нормальная ситуация.
    /// </summary>
    public HanoiSnapshot Seek(int moveIndex)
    {
        Reset();
        var target = Math.Clamp(moveIndex, 0, TotalMoves);

        for (var i = 0; i < target; i++)
        {
            Step();
        }

        return Snapshot();
    }
}
