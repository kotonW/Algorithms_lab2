using AlgorithmsLab2.Domain.Hanoi;
using Xunit;

namespace AlgorithmsLab2.Tests;

public sealed class HanoiPuzzleTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 3)]
    [InlineData(3, 7)]
    [InlineData(4, 15)]
    [InlineData(10, 1023)]
    public void Solver_UsesMinimalNumberOfMoves(int disks, int expected) =>
        Assert.Equal(expected, HanoiSolver.Solve(disks).Count);

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    public void Solution_IsLegalAndFinishesOnLastPeg(int disks)
    {
        var puzzle = new HanoiPuzzle(disks);

        while (!puzzle.IsComplete)
        {
            var before = puzzle.Snapshot();
            var move = puzzle.Moves[puzzle.MovesMade];

            // Со стержня снимается именно это кольцо — значит, оно верхнее.
            Assert.Equal(move.Disk, before.Pegs[move.FromPeg][^1]);

            // На стержень-приёмник можно класть только кольцо большего размера.
            if (before.Pegs[move.ToPeg].Count > 0)
            {
                Assert.True(
                    before.Pegs[move.ToPeg][^1] > move.Disk,
                    "Большее кольцо нельзя класть на меньшее.");
            }

            puzzle.Step();
        }

        var final = puzzle.Snapshot();

        // Стержни хранятся снизу вверх, поэтому на последнем стержне лежит n, n−1, …, 1.
        Assert.Equal(Enumerable.Range(1, disks).Reverse(), final.Pegs[2]);
        Assert.Empty(final.Pegs[0]);
    }

    [Fact]
    public void Reset_ReturnsToInitialState()
    {
        var puzzle = new HanoiPuzzle(4);
        puzzle.Step();
        puzzle.Step();

        var snapshot = puzzle.Reset();

        Assert.Equal(0, snapshot.MovesMade);
        Assert.Equal([4, 3, 2, 1], snapshot.Pegs[0]);
        Assert.Empty(snapshot.Pegs[1]);
        Assert.Empty(snapshot.Pegs[2]);
    }

    [Fact]
    public void StepBack_RestoresPreviousState()
    {
        var puzzle = new HanoiPuzzle(4);
        var initial = puzzle.Snapshot();

        puzzle.Step();
        puzzle.Step();
        var afterTwo = puzzle.Snapshot();

        var back = puzzle.Seek(1);

        Assert.Equal(1, back.MovesMade);
        Assert.NotEqual(afterTwo.Pegs[0], back.Pegs[0]);

        var backToStart = puzzle.Seek(0);
        Assert.Equal(initial.Pegs[0], backToStart.Pegs[0]);
        Assert.Equal(initial.MovesMade, backToStart.MovesMade);
    }

    [Fact]
    public void Snapshot_ReportsCounters()
    {
        var puzzle = new HanoiPuzzle(3);
        puzzle.Step();
        puzzle.Step();

        var snapshot = puzzle.Snapshot();

        Assert.Equal(7, snapshot.TotalMoves);
        Assert.Equal(2, snapshot.MovesMade);
        Assert.Equal(5, snapshot.MovesLeft);
        Assert.False(snapshot.IsComplete);
        Assert.NotNull(snapshot.LastMove);
    }

    [Fact]
    public void Solver_RejectsInvalidDiskCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => HanoiSolver.Solve(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new HanoiPuzzle(0));
    }
}
