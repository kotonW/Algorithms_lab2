using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Application.Canvases;
using AlgorithmsLab2.Application.Settings;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Центральная панель для замеров: хранит последний ряд данных и рисует его на холсте.
/// ScottPlot она не знает — работает только через <see cref="IBenchmarkCanvas"/>.
/// </summary>
public sealed class BenchmarkChartViewModel : IRedrawableTask
{
    private IBenchmarkCanvas? _canvas;
    private BenchmarkSeries _series;
    private bool _showTheory;

    public BenchmarkChartViewModel(BenchmarkRegistry registry, AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(settings);

        var algorithm = registry.Find(settings.BenchmarkAlgorithm) ?? registry.Default;

        _series = BenchmarkSeries.Empty(algorithm.Metadata);
        _showTheory = settings.ShowTheory;
    }

    /// <summary>Подключить холст; сразу рисует текущее состояние (пустой график или последний ряд).</summary>
    public void Attach(IBenchmarkCanvas canvas)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        _canvas.Show(_series, _showTheory);
    }

    /// <summary>Показать новый ряд данных.</summary>
    public void Show(BenchmarkSeries series, bool showTheory)
    {
        ArgumentNullException.ThrowIfNull(series);

        _series = series;
        _showTheory = showTheory;
        _canvas?.Show(_series, _showTheory);
    }

    /// <summary>Включить или выключить теоретическую кривую.</summary>
    public void SetTheoryVisible(bool showTheory) => Show(_series, showTheory);

    /// <summary>Очистить график (например, при смене алгоритма).</summary>
    public void Clear(AlgorithmMetadata metadata) =>
        Show(BenchmarkSeries.Empty(metadata), _showTheory);

    /// <summary>Смена темы: перекрашиваются фон, оси и сам график.</summary>
    public void Redraw() => _canvas?.RefreshTheme();

    /// <summary>Нажатие на график: показать подпись, если рядом с курсором есть точка.</summary>
    public void PickPoint(float x, float y)
    {
        if (_canvas is null)
        {
            return;
        }

        if (_canvas.TryPickPoint(x, y, out var point) && point is not null)
        {
            _canvas.ShowPointInfo(point);
        }
        else
        {
            _canvas.HidePointInfo();
        }
    }

    /// <summary>Кнопку отпустили или началось перетаскивание: подпись убираем.</summary>
    public void ReleasePoint() => _canvas?.HidePointInfo();
}
