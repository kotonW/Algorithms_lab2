using AlgorithmsLab2.Application.Apollonian;
using AlgorithmsLab2.Application.Canvases;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>Центральная панель аполлониевой решётки: перерисовывает холст при каждом изменении состояния.</summary>
public sealed class ApollonianPlotViewModel : IDisposable, IRedrawableTask
{
    private readonly ApollonianSession _session;

    private IApollonianCanvas? _canvas;

    public ApollonianPlotViewModel(ApollonianSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _session.Changed += OnSessionChanged;
    }

    /// <summary>Подключить холст; сразу рисует текущее состояние.</summary>
    public void Attach(IApollonianCanvas canvas)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        _canvas.Show(_session.Frame);
    }

    /// <summary>Перерисовать текущее состояние (например, после изменения размера окна).</summary>
    public void RequestRedraw() => _canvas?.Show(_session.Frame);

    /// <summary>Смена темы: фон тоже перекрашивается.</summary>
    public void Redraw() => RequestRedraw();

    /// <inheritdoc />
    public void Dispose() => _session.Changed -= OnSessionChanged;

    private void OnSessionChanged(object? sender, ApollonianFrame frame) => _canvas?.Show(frame);
}
