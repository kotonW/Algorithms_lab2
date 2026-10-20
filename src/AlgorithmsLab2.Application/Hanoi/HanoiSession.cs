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

    private static int Clamp(int diskCount) => Math.Clamp(diskCount, 1, 10);

    private HanoiSnapshot Publish(HanoiSnapshot snapshot)
    {
        Changed?.Invoke(this, snapshot);
        return snapshot;
    }
}
