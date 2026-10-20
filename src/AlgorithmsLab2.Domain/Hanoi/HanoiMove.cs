namespace AlgorithmsLab2.Domain.Hanoi;

/// <summary>Один перенос: кольцо <paramref name="Disk"/> со стержня <paramref name="FromPeg"/> на <paramref name="ToPeg"/>.</summary>
public readonly record struct HanoiMove(int Disk, int FromPeg, int ToPeg);
