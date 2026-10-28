using System.Globalization;
using AlgorithmsLab2.Application.Apollonian;
using AlgorithmsLab2.Application.Settings;
using AlgorithmsLab2.Domain.Apollonian;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Правая панель для аполлониевой решётки: глубина рекурсии, палитра, пошаговое построение и числа
/// (сколько кругов, какую долю внешнего круга они закрывают). Построение по шагам идёт по таймеру:
/// один уровень рекурсии за тик.
/// </summary>
public partial class ApollonianSettingsViewModel : ObservableObject, IDisposable
{
    /// <summary>Пауза между уровнями при пошаговом построении, мс.</summary>
    private const int LevelDelayMs = 700;

    private readonly ApollonianSession _session;
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    private int _depth;

    [ObservableProperty]
    private GasketPalette _selectedPalette;

    [ObservableProperty]
    private bool _isProgressive;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private string _circlesText = string.Empty;

    [ObservableProperty]
    private string _coverageText = string.Empty;

    [ObservableProperty]
    private string _uncoveredText = string.Empty;

    [ObservableProperty]
    private string _smallestRadiusText = string.Empty;

    [ObservableProperty]
    private string _levelText = string.Empty;

    public ApollonianSettingsViewModel(ApollonianSession session, AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(settings);

        _session = session;
        _depth = session.Depth;
        _selectedPalette = session.Palette;
        _isProgressive = settings.Progressive;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(LevelDelayMs) };
        _timer.Tick += OnTimerTick;

        _session.Changed += OnSessionChanged;

        UpdateInfo(_session.Frame);
    }

    public int MinDepth => 0;

    public int MaxDepth => ApolloniusGasket.MaxDepth;

    public IReadOnlyList<GasketPalette> Palettes => GasketPalettes.All;

    /// <summary>Хаусдорфова размерность решётки — постоянная, от глубины не зависит.</summary>
    public string DimensionText => "≈ 1,3057";

    /// <summary>Текст на кнопке: «Показать построение» или «Пауза».</summary>
    public string PlaybackText => IsPlaying ? "Пауза" : "Показать построение";

    /// <inheritdoc />
    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _session.Changed -= OnSessionChanged;
    }

    partial void OnDepthChanged(int value)
    {
        IsPlaying = false;

        // В пошаговом режиме решётка сразу «растёт» от нулевого уровня до выбранной глубины.
        _session.SetDepth(value, showAllLevels: !IsProgressive);

        if (IsProgressive && value > 0)
        {
            IsPlaying = true;
        }
    }

    partial void OnSelectedPaletteChanged(GasketPalette value) => _session.SetPalette(value);

    partial void OnIsPlayingChanged(bool value)
    {
        if (value)
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }

        OnPropertyChanged(nameof(PlaybackText));
    }

    [RelayCommand]
    private void TogglePlayback()
    {
        if (!IsPlaying && _session.VisibleLevels >= _session.Depth)
        {
            _session.SetVisibleLevels(0);
        }

        IsPlaying = !IsPlaying;
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _session.SetVisibleLevels(_session.VisibleLevels + 1);

        if (_session.VisibleLevels >= _session.Depth)
        {
            IsPlaying = false;
        }
    }

    private void OnSessionChanged(object? sender, ApollonianFrame frame) => UpdateInfo(frame);

    private void UpdateInfo(ApollonianFrame frame)
    {
        var statistics = frame.Statistics;

        CirclesText = statistics.CircleCount.ToString("N0", CultureInfo.CurrentCulture);
        CoverageText = statistics.Coverage.ToString("P3", CultureInfo.CurrentCulture);
        UncoveredText = statistics.Uncovered.ToString("P3", CultureInfo.CurrentCulture);
        SmallestRadiusText = statistics.SmallestRadius.ToString("E2", CultureInfo.CurrentCulture);
        LevelText = string.Create(
            CultureInfo.CurrentCulture,
            $"{statistics.VisibleLevels} из {frame.Gasket.Depth}");
    }
}
