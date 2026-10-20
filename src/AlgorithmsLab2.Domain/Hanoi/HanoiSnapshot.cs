namespace AlgorithmsLab2.Domain.Hanoi;

/// <summary>Мгновенное состояние головоломки: что лежит на каждом стержне и сколько ходов сделано.</summary>
public sealed record HanoiSnapshot(
    IReadOnlyList<IReadOnlyList<int>> Pegs,
    int DiskCount,
    int MovesMade,
    int TotalMoves,
    HanoiMove? LastMove)
{
    public bool IsComplete => MovesMade >= TotalMoves;

    /// <summary>Сколько ходов осталось до решения.</summary>
    public int MovesLeft => TotalMoves - MovesMade;
}
