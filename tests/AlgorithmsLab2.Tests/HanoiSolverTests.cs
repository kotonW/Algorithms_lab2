using AlgorithmsLab2.Domain.Hanoi;
using Xunit;

namespace AlgorithmsLab2.Tests;

public sealed class HanoiSolverTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    public void SolveWithStack_MatchesRecursiveSolution(int disks)
    {
        var recursive = HanoiSolver.Solve(disks);
        var stack = HanoiSolver.SolveWithStack(disks);

        Assert.Equal(recursive.Count, stack.Count);
        Assert.Equal(recursive, stack);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(10)]
    public void BothSolvers_ProduceMinimalNumberOfMoves(int disks)
    {
        var expected = (1 << disks) - 1;

        Assert.Equal(expected, HanoiSolver.Solve(disks).Count);
        Assert.Equal(expected, HanoiSolver.SolveWithStack(disks).Count);
    }

    [Fact]
    public void SolveWithStack_RejectsInvalidDiskCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => HanoiSolver.SolveWithStack(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => HanoiSolver.SolveWithStack(21));
    }

    [Fact]
    public void StackSolution_IsLegalForEveryMove()
    {
        var puzzle = new HanoiPuzzle(6);

        while (!puzzle.IsComplete)
        {
            var snapshot = puzzle.Snapshot();
            var move = puzzle.Moves[puzzle.MovesMade];

            Assert.Equal(move.Disk, snapshot.Pegs[move.FromPeg][^1]);

            if (snapshot.Pegs[move.ToPeg].Count > 0)
            {
                Assert.True(snapshot.Pegs[move.ToPeg][^1] > move.Disk);
            }

            puzzle.Step();
        }

        Assert.Equal(Enumerable.Range(1, 6).Reverse(), puzzle.Snapshot().Pegs[2]);
    }
}
