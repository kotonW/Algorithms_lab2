using AlgorithmsLab2.Application.Canvases;
using AlgorithmsLab2.Domain.Benchmarks;
using ScottPlot.Avalonia;

namespace AlgorithmsLab2.Rendering;

/// <summary>Мост между ScottPlot и ViewModel для дерева рекурсии. Вызывать только из UI-потока.</summary>
public sealed class AvaPlotTreeCanvas : IRecursionTreeCanvas
{
    private readonly AvaPlot _avaPlot;
    private readonly Func<bool> _isDark;

    private RecursionTree? _tree;

    /// <param name="isDark">Текущая тема: дерево подстраивает цвета под интерфейс.</param>
    public AvaPlotTreeCanvas(AvaPlot avaPlot, Func<bool>? isDark = null)
    {
        _avaPlot = avaPlot ?? throw new ArgumentNullException(nameof(avaPlot));
        _isDark = isDark ?? (() => true);

        // Красим фон сразу, чтобы на тёмной теме не было вспышки белого до первого кадра.
        TreePlotRenderer.Draw(_avaPlot.Plot, null, _isDark());
    }

    /// <inheritdoc />
    public void Show(RecursionTree? tree)
    {
        _tree = tree;
        TreePlotRenderer.Draw(_avaPlot.Plot, tree, _isDark());
        _avaPlot.Refresh();
    }

    /// <inheritdoc />
    public void RefreshTheme() => Show(_tree);
}
