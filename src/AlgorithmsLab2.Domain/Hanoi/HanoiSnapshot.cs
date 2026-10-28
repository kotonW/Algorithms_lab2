namespace AlgorithmsLab2.Domain.Hanoi;

/// <summary>Положение кольца в процессе плавной анимации перекладывания.</summary>
public readonly record struct HanoiMovingDisk(int Disk, double CenterX, double BottomY);

/// <summary>Мгновенное состояние головоломки: что лежит на каждом стержне и сколько ходов сделано.</summary>
public sealed record HanoiSnapshot(
    IReadOnlyList<IReadOnlyList<int>> Pegs,
    int DiskCount,
    int MovesMade,
    int TotalMoves,
    HanoiMove? LastMove,
    HanoiMovingDisk? MovingDisk = null)
{
    public bool IsComplete => MovesMade >= TotalMoves;

    /// <summary>Сколько ходов осталось до решения.</summary>
    public int MovesLeft => TotalMoves - MovesMade;
}