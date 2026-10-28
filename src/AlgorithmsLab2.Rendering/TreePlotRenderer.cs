using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Rendering;

/// <summary>
/// Рисует дерево рекурсии: корень сверху, вызовы — вниз. Цвет узла показывает порядок выполнения:
/// от синего (первый вызов) к оранжевому (последний). В небольших деревьях узлы подписаны,
/// в больших остаются только цветные точки — дерево можно приближать колесом мыши.
/// </summary>
public static class TreePlotRenderer
{
    /// <summary>До скольких узлов рисуем подписанные «карточки».</summary>
    private const int BoxLimit = 40;

    /// <summary>До скольких узлов точки крупные.</summary>
    private const int MediumLimit = 150;

    public static void Draw(ScottPlot.Plot plot, RecursionTree? tree, bool dark)
    {
        ArgumentNullException.ThrowIfNull(plot);

        var colors = PlotColors.For(dark);

        plot.Clear();
        plot.HideGrid();
        plot.Axes.Frameless();
        plot.FigureBackground.Color = colors.Background;
        plot.DataBackground.Color = colors.Background;

        if (tree is null)
        {
            plot.Axes.SetLimits(0, 1, 0, 1);
            return;
        }

        var positions = new Dictionary<RecursionNode, TreePosition>();
        var leafCount = 0;
        Layout(tree.Root, 0, ref leafCount, positions);

        var nodes = new List<RecursionNode>();
        Flatten(tree.Root, nodes);

        var maxDepth = positions.Values.Max(position => position.Depth);

        // Сначала рёбра, потом узлы: так узлы лежат поверх линий.
        foreach (var node in nodes)
        {
            var parentPosition = positions[node];

            foreach (var child in node.Children)
            {
                var childPosition = positions[child];
                var line = plot.Add.Line(
                    parentPosition.X,
                    -parentPosition.Depth,
                    childPosition.X,
                    -childPosition.Depth);
                line.LineWidth = 1.5f;
                line.Color = colors.Muted;
            }
        }

        var useBoxes = tree.NodeCount <= BoxLimit;
        var markerSize = tree.NodeCount <= MediumLimit ? 9f : 5f;

        foreach (var node in nodes)
        {
            var position = positions[node];
            var t = tree.NodeCount <= 1 ? 0.0 : (double)node.Order / (tree.NodeCount - 1);
            var fill = Lerp(colors.Measured, colors.Theory, t);

            if (useBoxes)
            {
                var text = plot.Add.Text(node.Label, position.X, -position.Depth);
                text.LabelFontSize = 11f;
                text.LabelBold = true;
                text.LabelFontColor = Contrast(fill);
                text.LabelBackgroundColor = fill;
                text.LabelPadding = 5;
                text.LabelAlignment = ScottPlot.Alignment.MiddleCenter;
            }
            else
            {
                plot.Add.Marker(position.X, -position.Depth, ScottPlot.MarkerShape.FilledCircle, markerSize, fill);
            }
        }

        var caption = $"Дерево рекурсии, {tree.Title}. Узлов: {tree.NodeCount}. "
                      + "Цвет — порядок выполнения: от синего (первый вызов) к оранжевому (последний)."
                      + (tree.IsTruncated ? " Показана только часть дерева." : string.Empty);

        var title = plot.Add.Text(caption, -0.7, 0.95);
        title.LabelFontSize = 12f;
        title.LabelFontColor = colors.Muted;
        title.LabelAlignment = ScottPlot.Alignment.UpperLeft;

        plot.Axes.SetLimits(-0.8, Math.Max(leafCount - 0.2, 0.8), -(maxDepth + 0.7), 1.0);
    }

    /// <summary>Листья расставляются подряд, родитель встаёт над серединой своих потомков.</summary>
    private static double Layout(
        RecursionNode node,
        int depth,
        ref int nextLeaf,
        Dictionary<RecursionNode, TreePosition> positions)
    {
        double x;

        if (node.Children.Count == 0)
        {
            x = nextLeaf;
            nextLeaf++;
        }
        else
        {
            var first = Layout(node.Children[0], depth + 1, ref nextLeaf, positions);
            var last = first;

            for (var i = 1; i < node.Children.Count; i++)
            {
                last = Layout(node.Children[i], depth + 1, ref nextLeaf, positions);
            }

            x = (first + last) / 2.0;
        }

        positions[node] = new TreePosition(x, depth);
        return x;
    }

    private static void Flatten(RecursionNode node, List<RecursionNode> result)
    {
        result.Add(node);

        foreach (var child in node.Children)
        {
            Flatten(child, result);
        }
    }

    private static ScottPlot.Color Lerp(ScottPlot.Color start, ScottPlot.Color end, double t) => new(
        (byte)(start.R + ((end.R - start.R) * t)),
        (byte)(start.G + ((end.G - start.G) * t)),
        (byte)(start.B + ((end.B - start.B) * t)));

    /// <summary>Цвет текста, читаемый на заданном фоне.</summary>
    private static ScottPlot.Color Contrast(ScottPlot.Color background)
    {
        var luminance = (0.299 * background.R) + (0.587 * background.G) + (0.114 * background.B);

        return luminance > 150
            ? new ScottPlot.Color(20, 22, 27)
            : new ScottPlot.Color(255, 255, 255);
    }

    private readonly record struct TreePosition(double X, int Depth);
}
