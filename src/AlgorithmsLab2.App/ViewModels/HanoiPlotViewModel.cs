using AlgorithmsLab2.Application.Canvases;
using AlgorithmsLab2.Application.Hanoi;
using AlgorithmsLab2.Domain.Hanoi;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>Центральная панель для башен: перерисовывает холст при каждом ходе.</summary>
public sealed class HanoiPlotViewModel : IDisposable
{
    private readonly HanoiSession _session;

    private IHanoiCanvas? _canvas;

    public HanoiPlotViewModel(HanoiSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _session.Changed += OnSessionChanged;
    }

    /// <summary>Подключить холст; сразу рисует текущее состояние.</summary>
    public void Attach(IHanoiCanvas canvas)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        _canvas.Show(_session.Snapshot);
    }

    /// <summary>Перерисовать текущее состояние (например, после изменения размера окна).</summary>
    public void RequestRedraw() => _canvas?.Show(_session.Snapshot);

    /// <inheritdoc />
    public void Dispose() => _session.Changed -= OnSessionChanged;

    private void OnSessionChanged(object? sender, HanoiSnapshot snapshot) => _canvas?.Show(snapshot);
}
