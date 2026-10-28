using AlgorithmsLab2.Domain.Hanoi;

namespace AlgorithmsLab2.Rendering;

/// <summary>
/// Рисует ханойские башни: три стержня на основании и кольца разной ширины.
/// Основание и стержни всегда одного размера — с ростом числа колец уменьшается
/// только толщина самих колец. Каждый кадр строится с нуля: элементов мало.
/// </summary>
public static class HanoiPlotRenderer
{
    private const double PegSpacing = 1.0;

    /// <summary>Высота стержня не зависит от числа колец — иначе «прыгает» масштаб.</summary>
    private const double PegHeight = 5.0;
    private const float PegLineWidth = 8f;

    private const double BaseLeft = -0.56;
    private const double BaseRight = 2.56;
    private const double BaseHeight = 0.16;

    private const double FirstDiskBottom = 0.14;
    private const double MaxDiskThickness = 0.62;
    private const double MaxDiskHalfWidth = 0.42;
    private const double MinDiskHalfWidth = 0.09;

    private const double SceneWidth = 3.5;
    private const double SceneBottom = -1.2;
    private const double SceneTop = PegHeight + 1;

    /// <param name="aspectRatio">Отношение ширины области рисования к высоте.</param>
    /// <param name="dark">Тёмная тема: от неё зависят фон и цвет стержней.</param>
    public static void Draw(ScottPlot.Plot plot, HanoiSnapshot snapshot, double aspectRatio, bool dark)
    {
        ArgumentNullException.ThrowIfNull(plot);
        ArgumentNullException.ThrowIfNull(snapshot);

        var colors = PlotColors.For(dark);

        plot.Clear();
        plot.HideGrid();
        plot.Axes.Frameless();
        plot.FigureBackground.Color = colors.Background;
        plot.DataBackground.Color = colors.Background;

        // Основание: скруглённая плита постоянного размера.
        DrawRoundedRectangle(
            plot,
            BaseLeft,
            BaseRight,
            -BaseHeight,
            0,
            0.06,
            colors.NeutralDark,
            Darken(colors.NeutralDark, 0.6));

        for (var peg = 0; peg < 3; peg++)
        {
            var line = plot.Add.Line(PegX(peg), 0, PegX(peg), PegHeight);
            line.LineWidth = PegLineWidth;
            line.Color = colors.Neutral;

            var label = plot.Add.Text(PegName(peg), PegX(peg), -0.72);
            label.LabelFontSize = 15f;
            label.LabelBold = true;
            label.LabelFontColor = colors.Muted;
        }

        DrawDisks(plot, snapshot);
        ApplyLimits(plot, aspectRatio);
    }

    private static void DrawDisks(ScottPlot.Plot plot, HanoiSnapshot snapshot)
    {
        var diskCount = Math.Max(snapshot.DiskCount, 1);

        // Свободная высота стержня делится на число колец: чем их больше, тем тоньше каждое.
        var usableHeight = PegHeight - FirstDiskBottom - 0.12;
        var thickness = Math.Min(MaxDiskThickness, usableHeight / diskCount * 0.78);
        var step = thickness / 0.78;

        for (var peg = 0; peg < 3; peg++)
        {
            var stack = snapshot.Pegs[peg];

            for (var level = 0; level < stack.Count; level++)
            {
                var disk = stack[level];
                DrawDiskAt(plot, PegX(peg), FirstDiskBottom + (level * step), disk, diskCount, thickness);
            }
        }

        // Если в кадре есть летящее кольцо — рисуем его поверх остальных
        if (snapshot.MovingDisk is { } moving)
        {
            DrawDiskAt(plot, moving.CenterX, moving.BottomY, moving.Disk, diskCount, thickness);
        }
    }

    private static void DrawDiskAt(
        ScottPlot.Plot plot,
        double centerX,
        double bottom,
        int disk,
        int diskCount,
        double thickness)
    {
        var halfWidth = MinDiskHalfWidth
                        + ((MaxDiskHalfWidth - MinDiskHalfWidth) * disk / diskCount);
        var fill = DiskColor(disk, diskCount);

        DrawRoundedRectangle(
            plot,
            centerX - halfWidth,
            centerX + halfWidth,
            bottom,
            bottom + thickness,
            Math.Min(thickness / 2.2, halfWidth / 2.5),
            fill,
            Darken(fill, 0.55));
    }

    /// <summary>Прямоугольник со скруглёнными углами: стандартные прямоугольники выглядят грубо.</summary>
    private static void DrawRoundedRectangle(
        ScottPlot.Plot plot,
        double left,
        double right,
        double bottom,
        double top,
        double radius,
        ScottPlot.Color fill,
        ScottPlot.Color outline)
    {
        const int stepsPerCorner = 5;

        var corner = Math.Max(0, Math.Min(radius, Math.Min((right - left) / 2.0, (top - bottom) / 2.0)));
        var xs = new List<double>();
        var ys = new List<double>();

        void Arc(double centerX, double centerY, double startAngle, double endAngle)
        {
            for (var step = 0; step <= stepsPerCorner; step++)
            {
                var angle = startAngle + ((endAngle - startAngle) * step / stepsPerCorner);
                xs.Add(centerX + (corner * Math.Cos(angle)));
                ys.Add(centerY + (corner * Math.Sin(angle)));
            }
        }

        Arc(right - corner, bottom + corner, -Math.PI / 2, 0);
        Arc(right - corner, top - corner, 0, Math.PI / 2);
        Arc(left + corner, top - corner, Math.PI / 2, Math.PI);
        Arc(left + corner, bottom + corner, Math.PI, 3 * Math.PI / 2);

        var polygon = plot.Add.Polygon(xs, ys);
        polygon.FillColor = fill;
        polygon.LineColor = outline;
        polygon.LineWidth = 1f;
    }

    private static void ApplyLimits(ScottPlot.Plot plot, double aspectRatio)
    {
        // Масштаб по осям одинаковый: иначе кольца выглядят приплюснутыми.
        // По более «длинной» оси добавляем поля, а не растягиваем сцену.
        var xRange = SceneWidth;
        var yRange = SceneTop - SceneBottom;
        var aspect = Math.Clamp(aspectRatio, 0.1, 10.0);

        if (aspect >= xRange / yRange)
        {
            xRange = yRange * aspect;
        }
        else
        {
            yRange = xRange / aspect;
        }

        const double centerX = PegSpacing;
        var centerY = (SceneBottom + SceneTop) / 2.0;

        plot.Axes.SetLimits(
            centerX - (xRange / 2.0),
            centerX + (xRange / 2.0),
            centerY - (yRange / 2.0),
            centerY + (yRange / 2.0));
    }

    private static double PegX(int peg) => peg * PegSpacing;

    private static string PegName(int peg) => peg switch
    {
        0 => "A",
        1 => "B",
        _ => "C",
    };

    /// <summary>Чем больше кольцо, тем насыщеннее цвет.</summary>
    private static ScottPlot.Color DiskColor(int disk, int diskCount)
    {
        var t = diskCount <= 1 ? 1.0 : (disk - 1) / (double)(diskCount - 1);

        return new ScottPlot.Color(
            (byte)(147 - (110 * t)),
            (byte)(200 - (100 * t)),
            (byte)(253 - (18 * t)));
    }

    private static ScottPlot.Color Darken(ScottPlot.Color color, double factor) => new(
        (byte)(color.R * factor),
        (byte)(color.G * factor),
        (byte)(color.B * factor));
}
