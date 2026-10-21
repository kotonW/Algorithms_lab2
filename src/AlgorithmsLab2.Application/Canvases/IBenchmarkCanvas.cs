using AlgorithmsLab2.Application.Benchmarks;

namespace AlgorithmsLab2.Application.Canvases;

/// <summary>Холст графика замеров. ViewModel работает только с этим интерфейсом и не знает про ScottPlot.</summary>
public interface IBenchmarkCanvas
{
    /// <summary>Рисует ряд измерений и, если нужно, теоретическую кривую.</summary>
    void Show(BenchmarkSeries series, bool showTheory);

    /// <summary>Перекрашивает график под текущую тему.</summary>
    void RefreshTheme();

    /// <summary>
    /// Ищет точку рядом с позицией курсора (координаты в DIP относительно графика).
    /// Возвращает <c>false</c>, если рядом с курсором точки нет.
    /// </summary>
    bool TryPickPoint(float x, float y, out BenchmarkPoint? point);

    /// <summary>Показывает подпись с координатами точки.</summary>
    void ShowPointInfo(BenchmarkPoint point);

    /// <summary>Убирает подпись.</summary>
    void HidePointInfo();
}
