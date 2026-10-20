using AlgorithmsLab2.Application.Canvases;
using AlgorithmsLab2.Application.Rendering;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Центральная панель для ковра Серпинского: по изменению настроек перестраивает кадр
/// в фоне и отдаёт готовый растр на холст.
/// </summary>
public sealed class SierpinskiPlotViewModel : ObservableObject, IDisposable
{
    private readonly IFractalRenderer _renderer;
    private readonly SierpinskiSettingsViewModel _settings;

    private IFractalCanvas? _canvas;
    private CancellationTokenSource? _cts;
    private bool _disposed;

    public SierpinskiPlotViewModel(IFractalRenderer renderer, SierpinskiSettingsViewModel settings)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        _settings.SettingsChanged += OnSettingsChanged;
    }

    /// <summary>Подключить холст (вызывается из View, когда контрол появился на экране).</summary>
    public void Attach(IFractalCanvas canvas)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        _ = RefreshAsync();
    }

    /// <summary>Перерисовать кадр: например, после изменения размера окна.</summary>
    public void RequestRedraw() => _ = RefreshAsync();

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _settings.SettingsChanged -= OnSettingsChanged;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private void OnSettingsChanged(object? sender, EventArgs e) => _ = RefreshAsync();

    private async Task RefreshAsync()
    {
        var canvas = _canvas;

        if (canvas is null || _disposed)
        {
            return;
        }

        // Новый запрос отменяет предыдущий: ползунок глубины генерирует их десятками.
        var previous = _cts;
        var cts = new CancellationTokenSource();
        _cts = cts;
        previous?.Cancel();
        previous?.Dispose();

        var token = cts.Token;

        // Настройки читаем один раз, до первого await: потом их могут изменить.
        var depth = RenderLimits.ClampDepth(_settings.Depth);
        var palette = _settings.SelectedPalette;
        var progressive = _settings.IsProgressive;

        try
        {
            // Небольшая пауза сглаживает перетаскивание ползунка.
            await Task.Delay(progressive ? 20 : 90, token).ConfigureAwait(true);

            if (progressive)
            {
                await PlayGrowthAsync(canvas, depth, palette, token);
            }
            else
            {
                var request = SierpinskiRenderRequest.ForDepth(depth);
                var frame = await Task.Run(() => _renderer.Render(request, token), token);

                token.ThrowIfCancellationRequested();

                canvas.Show(frame.Raster, palette);
                _settings.ApplyStatistics(frame.Statistics);
            }
        }
        catch (OperationCanceledException)
        {
            // Отменённый кадр просто не показываем — это нормальный сценарий.
        }
    }

    /// <summary>Пункт F10: показываем, как фрактал «растёт» от нулевой глубины к заданной.</summary>
    private async Task PlayGrowthAsync(
        IFractalCanvas canvas,
        int depth,
        Application.Painting.FractalPalette palette,
        CancellationToken token)
    {
        for (var level = 0; level <= depth; level++)
        {
            var request = SierpinskiRenderRequest.ForDepth(level);
            var frame = await Task.Run(() => _renderer.Render(request, token), token);

            token.ThrowIfCancellationRequested();

            canvas.Show(frame.Raster, palette);

            if (level == depth)
            {
                _settings.ApplyStatistics(frame.Statistics);
            }

            await Task.Delay(180, token).ConfigureAwait(true);
        }
    }
}
