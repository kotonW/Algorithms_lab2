using AlgorithmsLab2.Application.Hanoi;
using AlgorithmsLab2.Domain.Hanoi;
using Xunit;

namespace AlgorithmsLab2.Tests;

public sealed class HanoiSessionTests
{
    [Fact]
    public void StepBack_AtStart_DoesNothingAndDoesNotThrow()
    {
        var session = new HanoiSession(4);

        var snapshot = session.StepBack();

        Assert.Equal(0, snapshot.MovesMade);
        Assert.Equal([4, 3, 2, 1], snapshot.Pegs[0]);
        Assert.Null(snapshot.LastMove);
    }

    [Fact]
    public void StepBack_AtStart_DoesNotRaiseChanged()
    {
        var session = new HanoiSession(3);
        var raised = 0;
        session.Changed += (_, _) => raised++;

        session.StepBack();
        session.StepBack();

        Assert.Equal(0, raised);
    }

    [Fact]
    public void StepBack_AfterMoves_ReturnsToPreviousState()
    {
        var session = new HanoiSession(3);
        var initial = session.Snapshot;

        session.Step();
        session.Step();
        var afterTwo = session.Snapshot;

        var back = session.StepBack();

        Assert.Equal(afterTwo.MovesMade - 1, back.MovesMade);
        Assert.NotEqual(afterTwo.Pegs[0], back.Pegs[0]);

        session.StepBack();
        var start = session.Snapshot;

        Assert.Equal(initial.Pegs[0], start.Pegs[0]);
        Assert.Equal(0, start.MovesMade);
    }

    [Fact]
    public void Resize_ChangesDiskCountAndResetsProgress()
    {
        var session = new HanoiSession(3);
        session.Step();
        session.Step();

        var snapshot = session.Resize(5);

        Assert.Equal(5, snapshot.DiskCount);
        Assert.Equal(0, snapshot.MovesMade);
        Assert.Equal(31, snapshot.TotalMoves);
    }

    [Fact]
    public void Seek_ClampsOutOfRangeIndices()
    {
        var puzzle = new HanoiPuzzle(3);

        Assert.Equal(0, puzzle.Seek(-5).MovesMade);
        Assert.Equal(puzzle.TotalMoves, puzzle.Seek(9999).MovesMade);
    }
}
