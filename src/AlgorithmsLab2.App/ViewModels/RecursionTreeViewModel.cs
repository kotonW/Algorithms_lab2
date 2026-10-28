using AlgorithmsLab2.Application.Canvases;
using AlgorithmsLab2.Domain.Benchmarks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>Панель дерева рекурсии под графиком замеров: хранит текущее дерево и рисует его на холсте.</summary>
public sealed partial class RecursionTreeViewModel : ObservableObject
{
    private IRecursionTreeCanvas? _canvas;
    private RecursionTree? _tree;

    /// <summary>Показана ли панель. По умолчанию дерево видно.</summary>
    [ObservableProperty]
    private bool _isVisible = true;

    /// <summary>Подключить холст; сразу рисует текущее дерево.</summary>
    public void Attach(IRecursionTreeCanvas canvas)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        _canvas.Show(_tree);
    }

    /// <summary>Показать новое дерево; <c>null</c> очищает панель.</summary>
    public void Show(RecursionTree? tree)
    {
        _tree = tree;
        _canvas?.Show(tree);
    }

    /// <summary>Смена темы: дерево перекрашивается.</summary>
    public void Redraw() => _canvas?.RefreshTheme();
}
