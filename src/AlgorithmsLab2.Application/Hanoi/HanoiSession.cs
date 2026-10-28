using AlgorithmsLab2.Domain.Hanoi;

namespace AlgorithmsLab2.Application.Hanoi;

/// <summary>
/// Одна головоломка на всё приложение: хранит состояние, умеет шагать вперёд-назад
/// и сообщает об изменениях обоим ViewModel (панель настроек и холст).
/// </summary>
public sealed class HanoiSession
{
    private HanoiPuzzle _puzzle;

    public HanoiSession(int diskCount) => _puzzle = new HanoiPuzzle(Clamp(diskCount));

    /// <summary>Состояние изменилось: аргумент — новый снимок.</summary>
    public event EventHandler<HanoiSnapshot>? Changed;

    public int DiskCount => _puzzle.DiskCount;

    public int TotalMoves => _puzzle.TotalMoves;

    public int MovesMade => _puzzle.MovesMade;

    public bool IsComplete => _puzzle.IsComplete;

    public HanoiSnapshot Snapshot => _puzzle.Snapshot();

    /// <summary>Следующий ход, который предстоит сделать (если головоломка ещё не решена).</summary>
    public HanoiMove? NextMove => MovesMade < TotalMoves ? _puzzle.Moves[MovesMade] : null;

    /// <summary>Меняет число колец (с пересозданием головоломки).</summary>
    public HanoiSnapshot Resize(int diskCount)
    {
        _puzzle = new HanoiPuzzle(Clamp(diskCount));
        return Publish(_puzzle.Snapshot());
    }

    public HanoiSnapshot Step() => Publish(_puzzle.Step());

    /// <summary>
    /// Ход назад. Если ходов ещё не было, ничего не происходит — это не ошибка.
    /// </summary>
    public HanoiSnapshot StepBack()
    {
        if (_puzzle.MovesMade == 0)
        {
            return _puzzle.Snapshot();
        }

        return Publish(_puzzle.Seek(_puzzle.MovesMade - 1));
    }

    public HanoiSnapshot Reset() => Publish(_puzzle.Reset());

    public HanoiSnapshot Seek(int moveIndex) => Publish(_puzzle.Seek(moveIndex));

    /// <summary>
    /// Публикует промежуточный кадр анимации: верхнее кольцо временно снято со стержня
    /// и отображается по заданным координатам (x, y).
    /// </summary>
    public void AnimateFrame(int disk, int sourcePeg, double x, double y)
    {
        var current = _puzzle.Snapshot();
        var tempPegs = current.Pegs
            .Select((peg, i) => i == sourcePeg && peg.Count > 0
                ? (IReadOnlyList<int>)peg.Take(peg.Count - 1).ToArray()
                : peg)
            .ToArray();

        var frameSnapshot = new HanoiSnapshot(
            tempPegs,
            _puzzle.DiskCount,
            _puzzle.MovesMade,
            _puzzle.TotalMoves,
            current.LastMove,
            new HanoiMovingDisk(disk, x, y));

        Changed?.Invoke(this, frameSnapshot);
    }

    private static int Clamp(int diskCount) => Math.Clamp(diskCount, 1, 10);

    private HanoiSnapshot Publish(HanoiSnapshot snapshot)
    {
        Changed?.Invoke(this, snapshot);
        return snapshot;
    }
}