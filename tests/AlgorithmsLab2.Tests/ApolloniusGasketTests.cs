using AlgorithmsLab2.Application.Apollonian;
using AlgorithmsLab2.Domain.Apollonian;
using Xunit;

namespace AlgorithmsLab2.Tests;

public class ApolloniusGasketTests
{
    private const double Tolerance = 1e-9;

    private static long Pow3(int power)
    {
        long result = 1;

        for (var i = 0; i < power; i++)
        {
            result *= 3;
        }

        return result;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    public void Build_ProducesTwoTimesThreeToTheNPlusOneCircles(int depth)
    {
        var gasket = ApolloniusGasket.Build(depth);

        Assert.Equal((2 * Pow3(depth)) + 1, gasket.Circles.Count);
        Assert.Equal(gasket.Circles.Count, ApolloniusGasket.CircleCount(depth));
        Assert.Equal(depth, gasket.Depth);
    }

    [Fact]
    public void Build_LevelJHasFourTimesThreeToTheJMinusOneCircles()
    {
        var gasket = ApolloniusGasket.Build(5);
        var byLevel = gasket.Circles.GroupBy(circle => circle.Level).ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(3, byLevel[0]);

        for (var level = 1; level <= 5; level++)
        {
            Assert.Equal(4 * Pow3(level - 1), byLevel[level]);
        }
    }

    [Fact]
    public void Build_StartsWithThreeEqualCirclesOfKnownRadius()
    {
        var baseCircles = ApolloniusGasket.Build(0).Circles;
        var expectedRadius = (2 * Math.Sqrt(3)) - 3;

        Assert.Equal(3, baseCircles.Count);
        Assert.All(baseCircles, circle => Assert.Equal(expectedRadius, circle.Radius, 9));
    }

    [Fact]
    public void Build_AllCirclesLieInsideOuterCircleAndNeverOverlap()
    {
        var circles = ApolloniusGasket.Build(4).Circles;

        for (var i = 0; i < circles.Count; i++)
        {
            var a = circles[i];
            Assert.True(Math.Sqrt((a.X * a.X) + (a.Y * a.Y)) + a.Radius <= ApolloniusGasket.OuterRadius + Tolerance);

            for (var j = i + 1; j < circles.Count; j++)
            {
                var b = circles[j];
                var distance = Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

                Assert.True(distance >= a.Radius + b.Radius - Tolerance, $"круги {i} и {j} пересекаются");
            }
        }
    }

    [Fact]
    public void Build_EveryGeneratedCircleTouchesAtLeastThreeOthers()
    {
        var circles = ApolloniusGasket.Build(4).Circles;

        for (var i = 0; i < circles.Count; i++)
        {
            var a = circles[i];

            if (a.Level == 0)
            {
                continue;
            }

            var touching = 0;

            // Касание внешней окружности изнутри.
            if (Math.Abs(Math.Sqrt((a.X * a.X) + (a.Y * a.Y)) + a.Radius - ApolloniusGasket.OuterRadius) < Tolerance)
            {
                touching++;
            }

            for (var j = 0; j < circles.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var b = circles[j];
                var distance = Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

                if (Math.Abs(distance - (a.Radius + b.Radius)) < Tolerance)
                {
                    touching++;
                }
            }

            Assert.True(touching >= 3, $"круг {i} касается только {touching} кругов");
        }
    }

    [Fact]
    public void Build_CoverageGrowsTowardsWholeOuterCircle()
    {
        var previous = 0.0;

        for (var depth = 0; depth <= ApolloniusGasket.MaxDepth; depth++)
        {
            var coverage = GasketStatistics.For(ApolloniusGasket.Build(depth), depth).Coverage;

            Assert.True(coverage > previous, $"покрытие не растёт на глубине {depth}");
            Assert.True(coverage < 1.0);
            previous = coverage;
        }

        Assert.True(previous > 0.99);
    }

    [Fact]
    public void Statistics_AtDepthZeroMatchesThreeBaseCircles()
    {
        var statistics = GasketStatistics.For(ApolloniusGasket.Build(0), 0);

        Assert.Equal(3, statistics.CircleCount);
        Assert.Equal(63 - (36 * Math.Sqrt(3)), statistics.Coverage, 9);
        Assert.Equal((2 * Math.Sqrt(3)) - 3, statistics.SmallestRadius, 9);
        Assert.Equal(1.0 - statistics.Coverage, statistics.Uncovered, 12);
    }

    [Fact]
    public void Statistics_CountsOnlyVisibleLevels()
    {
        var gasket = ApolloniusGasket.Build(3);

        Assert.Equal(7, GasketStatistics.For(gasket, 1).CircleCount);
        Assert.Equal(19, GasketStatistics.For(gasket, 2).CircleCount);
        Assert.Equal(55, GasketStatistics.For(gasket, 3).CircleCount);
        Assert.Equal(55, GasketStatistics.For(gasket, 99).CircleCount);
        Assert.Equal(3, GasketStatistics.For(gasket, -5).CircleCount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(ApolloniusGasket.MaxDepth + 1)]
    public void Build_OutOfRange_Throws(int depth)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ApolloniusGasket.Build(depth));
        Assert.Throws<ArgumentOutOfRangeException>(() => ApolloniusGasket.CircleCount(depth));
    }

    [Fact]
    public void Palette_InterpolatesBetweenStops()
    {
        var palette = new GasketPalette("Тест", new GasketRgb(0, 0, 0), new GasketRgb(100, 200, 50));

        Assert.Equal(new GasketRgb(0, 0, 0), palette.ColorAt(0));
        Assert.Equal(new GasketRgb(100, 200, 50), palette.ColorAt(1));
        Assert.Equal(new GasketRgb(50, 100, 25), palette.ColorAt(0.5));
        Assert.Equal(new GasketRgb(100, 200, 50), palette.ColorAt(7));
    }

    [Fact]
    public void Palette_RequiresAtLeastTwoStops()
    {
        Assert.Throws<ArgumentException>(() => new GasketPalette("Один", new GasketRgb(1, 2, 3)));
    }

    [Fact]
    public void Palettes_HaveUniqueNamesAndUnknownFallsBackToDefault()
    {
        var names = GasketPalettes.All.Select(palette => palette.Name).ToList();

        Assert.Equal(names.Count, names.Distinct().Count());
        Assert.Same(GasketPalettes.Default, GasketPalettes.Find("такой нет"));
        Assert.Same(GasketPalettes.Default, GasketPalettes.Find(null));
        Assert.Same(GasketPalettes.All[1], GasketPalettes.Find(GasketPalettes.All[1].Name));
    }

    [Fact]
    public void Session_StartsFullyBuiltAndRaisesChangedOnUpdates()
    {
        var session = new ApollonianSession(3, GasketPalettes.Default);
        var raised = 0;
        session.Changed += (_, _) => raised++;

        Assert.Equal(3, session.Depth);
        Assert.Equal(3, session.VisibleLevels);

        session.SetDepth(5);
        Assert.Equal(5, session.Depth);
        Assert.Equal(5, session.VisibleLevels);
        Assert.Equal(1, raised);

        session.SetDepth(5);
        Assert.Equal(1, raised);

        session.SetPalette(GasketPalettes.All[1]);
        Assert.Same(GasketPalettes.All[1], session.Palette);
        Assert.Equal(2, raised);
    }

    [Fact]
    public void Session_VisibleLevelsAreClampedAndRaiseOnlyOnChange()
    {
        var session = new ApollonianSession(4, GasketPalettes.Default);
        var raised = 0;
        session.Changed += (_, _) => raised++;

        session.SetVisibleLevels(99);
        Assert.Equal(4, session.VisibleLevels);
        Assert.Equal(0, raised);

        session.SetVisibleLevels(-3);
        Assert.Equal(0, session.VisibleLevels);
        Assert.Equal(1, raised);

        session.SetVisibleLevels(2);
        Assert.Equal(2, session.Frame.VisibleLevels);
        Assert.Equal(2, raised);
    }

    [Fact]
    public void Session_StepByStepDepthChangeStartsFromLevelZero()
    {
        var session = new ApollonianSession(2, GasketPalettes.Default);

        session.SetDepth(4, showAllLevels: false);

        Assert.Equal(4, session.Depth);
        Assert.Equal(0, session.VisibleLevels);
        Assert.Equal(3, session.Frame.Statistics.CircleCount);
    }
}
