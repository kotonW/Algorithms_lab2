using AlgorithmsLab2.Application.Apollonian;

namespace AlgorithmsLab2.Application.Canvases;

/// <summary>Холст аполлониевой решётки. ViewModel работает только с этим интерфейсом и не знает про ScottPlot.</summary>
public interface IApollonianCanvas
{
    /// <summary>Рисует кадр.</summary>
    void Show(ApollonianFrame frame);
}
