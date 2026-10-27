using System.Collections.ObjectModel;
using System.Globalization;
using AlgorithmsLab2.Application.Hanoi;
using AlgorithmsLab2.Application.Rendering;
using AlgorithmsLab2.Application.Settings;
using AlgorithmsLab2.Domain.Hanoi;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Правая панель для ханойских башен: число колец, скорость и счётчики ходов.
/// Проигрывание идёт по таймеру, один ход за тик.
/// </summary>
public partial class HanoiSettingsViewModel : ObservableObject, IDisposable
{
    /// <summary>Наименьшая задержка между ходами, мс.</summary>
    public const int MinStepDelayMs = 20;

    /// <summary>Наибольшая задержка между ходами, мс (2 секунды).</summary>
    public const int MaxStepDelayMs = 2000;

    private static readonly string[] PegNames = ["A", "B", "C"];

    private readonly HanoiSession _session;
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    private int _diskCount;

    [ObservableProperty]
    private int _stepDelayMs;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private string _totalMovesText = string.Empty;

    [ObservableProperty]
    private string _movesMadeText = string.Empty;

    [ObservableProperty]
    private string _movesLeftText = string.Empty;

    [ObservableProperty]
    private string _currentMoveText = "—";

    [ObservableProperty]
    private string _progressText = "0 %";

    [ObservableProperty]
    private string _benchmarkText = "Нажмите кнопку, чтобы сравнить рекурсию и стек.";

    [ObservableProperty]
    private int _logSelectedIndex = -1;

    public HanoiSettingsViewModel(HanoiSession session, AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(settings);

        _session = session;
        _diskCount = Math.Clamp(settings.DiskCount, RenderLimits.MinHanoiDisks, RenderLimits.MaxHanoiDisks);
        _stepDelayMs = Math.Clamp(settings.StepDelayMs, MinStepDelayMs, MaxStepDelayMs);

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(_stepDelayMs) };
        _timer.Tick += OnTimerTick;

        _session.Changed += OnSessionChanged;

        // Приводим головоломку к сохранённому числу колец.
        _session.Resize(_diskCount);
        UpdateCounters(_session.Snapshot);
    }

    public int MinDisks => RenderLimits.MinHanoiDisks;

    public int MaxDisks => RenderLimits.MaxHanoiDisks;

    public int MinDelayMs => MinStepDelayMs;

    public int MaxDelayMs => MaxStepDelayMs;

    /// <summary>Журнал сделанных ходов по порядку; шаг назад и сброс убирают записи.</summary>
    public ObservableCollection<HanoiLogEntry> MoveLog { get; } = [];

    /// <summary>Текст на кнопке запуска: «Провести симуляцию» или «Пауза».</summary>
    public string PlaybackText => IsPlaying ? "Пауза" : "Провести симуляцию";

    /// <inheritdoc />
    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _session.Changed -= OnSessionChanged;
    }

    partial void OnDiskCountChanged(int value)
    {
        StopPlayback();
        _session.Resize(value);
        UpdateCounters(_session.Snapshot);
    }

    partial void OnStepDelayMsChanged(int value) =>
        _timer.Interval = TimeSpan.FromMilliseconds(Math.Clamp(value, MinStepDelayMs, MaxStepDelayMs));

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
        if (_session.IsComplete && !IsPlaying)
        {
            _session.Reset();
        }

        IsPlaying = !IsPlaying;
    }

    [RelayCommand]
    private void StepForward()
    {
        IsPlaying = false;
        _session.Step();
    }

    [RelayCommand]
    private void StepBack()
    {
        IsPlaying = false;
        _session.StepBack();
    }

    [RelayCommand]
    private void Reset()
    {
        IsPlaying = false;
        _session.Reset();
    }

    /// <summary>
    /// Сравнение двух способов получить последовательность ходов: рекурсии и явного стека.
    /// </summary>
    [RelayCommand]
    private void RunBenchmark()
    {
        var result = HanoiBenchmark.Run(DiskCount);

        BenchmarkText =
            $"Колец {result.DiskCount}, ходов {result.Moves:N0}.\n"
            + $"Рекурсия: {result.RecursiveMicroseconds:F1} мкс\n"
            + $"Стек: {result.StackMicroseconds:F1} мкс\n"
            + (result.SameMoves ? "Последовательности ходов совпадают." : "Последовательности различаются!");
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_session.IsComplete)
        {
            IsPlaying = false;
            return;
        }

        _session.Step();
    }

    private void OnSessionChanged(object? sender, HanoiSnapshot snapshot) => UpdateCounters(snapshot);

    private void StopPlayback() => IsPlaying = false;

    private void UpdateCounters(HanoiSnapshot snapshot)
    {
        TotalMovesText = snapshot.TotalMoves.ToString("N0", CultureInfo.CurrentCulture);
        MovesMadeText = snapshot.MovesMade.ToString("N0", CultureInfo.CurrentCulture);
        MovesLeftText = snapshot.MovesLeft.ToString("N0", CultureInfo.CurrentCulture);

        ProgressText = snapshot.TotalMoves == 0
            ? "0 %"
            : (snapshot.MovesMade / (double)snapshot.TotalMoves).ToString("P0", CultureInfo.CurrentCulture);

        CurrentMoveText = snapshot.LastMove is { } move
            ? $"Кольцо {move.Disk}: {PegNames[move.FromPeg]} → {PegNames[move.ToPeg]}"
            : "—";

        SyncLog(snapshot);
    }

    /// <summary>
    /// Приводит журнал в соответствие с состоянием головоломки. Ход вперёд добавляет запись,
    /// шаг назад, сброс и смена числа колец убирают записи, которых больше нет.
    /// </summary>
    private void SyncLog(HanoiSnapshot snapshot)
    {
        var made = snapshot.MovesMade;

        if (made == 0)
        {
            MoveLog.Clear();
        }
        else
        {
            while (MoveLog.Count > made)
            {
                MoveLog.RemoveAt(MoveLog.Count - 1);
            }
        }

        if (MoveLog.Count == made - 1 && snapshot.LastMove is { } move)
        {
            MoveLog.Add(new HanoiLogEntry(made, move.Disk, PegNames[move.FromPeg], PegNames[move.ToPeg]));
        }

        // Выделяем последнюю запись: список сам прокручивается к текущему ходу.
        LogSelectedIndex = MoveLog.Count - 1;
    }
}
