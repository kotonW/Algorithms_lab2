using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using AlgorithmsLab2.Application.Hanoi;
using AlgorithmsLab2.Application.Rendering;
using AlgorithmsLab2.Application.Settings;
using AlgorithmsLab2.Domain.Hanoi;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Правая панель для ханойских башен: число колец, скорость и счётчики ходов.
/// Поддерживает плавную 60 FPS анимацию перемещения колец по воздуху.
/// </summary>
public partial class HanoiSettingsViewModel : ObservableObject, IDisposable
{
    public const int MinStepDelayMs = 20;
    public const int MaxStepDelayMs = 2000;

    private static readonly string[] PegNames = ["A", "B", "C"];

    private readonly HanoiSession _session;
    private bool _isAnimating;

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

        _session.Changed += OnSessionChanged;

        _session.Resize(_diskCount);
        UpdateCounters(_session.Snapshot);
    }

    public int MinDisks => RenderLimits.MinHanoiDisks;

    public int MaxDisks => RenderLimits.MaxHanoiDisks;

    public int MinDelayMs => MinStepDelayMs;

    public int MaxDelayMs => MaxStepDelayMs;

    public ObservableCollection<HanoiLogEntry> MoveLog { get; } = [];

    public string PlaybackText => IsPlaying ? "Пауза" : "Провести симуляцию";

    public void Dispose()
    {
        _session.Changed -= OnSessionChanged;
    }

    partial void OnDiskCountChanged(int value)
    {
        IsPlaying = false;
        _session.Resize(value);
        UpdateCounters(_session.Snapshot);
    }

    partial void OnIsPlayingChanged(bool value)
    {
        if (value)
        {
            _ = PlaybackLoopAsync();
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
    private async Task StepForward()
    {
        if (_isAnimating || _session.IsComplete) return;
        IsPlaying = false;
        await AnimateStepAsync(forward: true);
    }

    [RelayCommand]
    private async Task StepBack()
    {
        if (_isAnimating || _session.MovesMade == 0) return;
        IsPlaying = false;
        await AnimateStepAsync(forward: false);
    }

    [RelayCommand]
    private void Reset()
    {
        IsPlaying = false;
        _session.Reset();
    }

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

    private async Task PlaybackLoopAsync()
    {
        while (IsPlaying && !_session.IsComplete)
        {
            await AnimateStepAsync(forward: true);

            // Небольшая пауза между шагами, зависящая от ползунка скорости
            var pause = Math.Max(10, StepDelayMs / 6);
            await Task.Delay(pause);
        }

        if (_session.IsComplete)
        {
            IsPlaying = false;
        }
    }

    /// <summary>
    /// Плавная 3-фазная анимация одного хода (подъём, полёт по воздуху, спуск).
    /// </summary>
    private async Task AnimateStepAsync(bool forward)
    {
        if (_isAnimating) return;

        HanoiMove move;
        int fromPeg, toPeg;

        if (forward)
        {
            if (_session.NextMove is not { } next) return;
            move = next;
            fromPeg = move.FromPeg;
            toPeg = move.ToPeg;
        }
        else
        {
            if (_session.Snapshot.LastMove is not { } prev) return;
            move = prev;
            fromPeg = move.ToPeg;
            toPeg = move.FromPeg;
        }

        // Если задержка минимальна (< 60 мс), делаем мгновенный шаг без анимации
        if (StepDelayMs < 60)
        {
            if (forward) _session.Step(); else _session.StepBack();
            return;
        }

        _isAnimating = true;

        try
        {
            var diskCount = Math.Max(_session.DiskCount, 1);
            var usableHeight = 5.0 - 0.14 - 0.12;
            var thickness = Math.Min(0.62, usableHeight / diskCount * 0.78);
            var step = thickness / 0.78;

            var snapshot = _session.Snapshot;
            var fromCount = snapshot.Pegs[fromPeg].Count;
            var toCount = snapshot.Pegs[toPeg].Count;

            var startX = fromPeg * 1.0;
            var endX = toPeg * 1.0;
            var startY = 0.14 + ((fromCount - 1) * step);
            var targetY = 0.14 + (toCount * step);
            const double topY = 5.08; // Чуть выше верхушки стержня (PegHeight = 5.0)

            var duration = Math.Clamp(StepDelayMs * 0.8, 50, 700);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.ElapsedMilliseconds < duration)
            {
                var progress = Math.Clamp(stopwatch.ElapsedMilliseconds / duration, 0.0, 1.0);
                double currentX, currentY;

                // Фаза 1: подъём (0.0 .. 0.33)
                if (progress < 0.33)
                {
                    var p = progress / 0.33;
                    var ease = (1 - Math.Cos(p * Math.PI)) / 2;
                    currentX = startX;
                    currentY = startY + ((topY - startY) * ease);
                }
                // Фаза 2: горизонтальный перелёт (0.33 .. 0.67)
                else if (progress < 0.67)
                {
                    var p = (progress - 0.33) / 0.34;
                    var ease = (1 - Math.Cos(p * Math.PI)) / 2;
                    currentX = startX + ((endX - startX) * ease);
                    currentY = topY;
                }
                // Фаза 3: опускание (0.67 .. 1.0)
                else
                {
                    var p = (progress - 0.67) / 0.33;
                    var ease = (1 - Math.Cos(p * Math.PI)) / 2;
                    currentX = endX;
                    currentY = topY + ((targetY - topY) * ease);
                }

                _session.AnimateFrame(move.Disk, fromPeg, currentX, currentY);
                await Task.Delay(16); // ~60 FPS
            }

            // Завершаем ход в доменной модели
            if (forward) _session.Step(); else _session.StepBack();
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private void OnSessionChanged(object? sender, HanoiSnapshot snapshot)
    {
        // Не трогаем счётчики во время промежуточных кадров анимации
        if (snapshot.MovingDisk is null)
        {
            UpdateCounters(snapshot);
        }
    }

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

        LogSelectedIndex = MoveLog.Count - 1;
    }
}