using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Application.Canvases;

/// <summary>Холст дерева рекурсии. ViewModel работает только с этим интерфейсом и не знает про ScottPlot.</summary>
public interface IRecursionTreeCanvas
{
    /// <summary>Рисует дерево; <c>null</c> — пустой холст.</summary>
    void Show(RecursionTree? tree);

    /// <summary>Перекрашивает дерево под текущую тему.</summary>
    void RefreshTheme();
}
