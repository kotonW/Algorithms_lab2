using System.Collections.ObjectModel;
using System.Globalization;
using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Application.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Правая панель для замеров: выбор алгоритма, параметры эксперимента, запуск и отмена,
/// оценка длительности и таблица точек. Сам замер идёт в фоновом потоке.
/// </summary>
public partial class BenchmarkSettingsViewModel : ObservableObject, IDisposable
{
    private readonly BenchmarkRegistry _registry;
    private readonly BenchmarkRunner _runner;
    private readonly BenchmarkChartViewModel _chart;

    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private IAlgorithmBenchmark _selectedAlgorithm;

    [ObservableProperty]
    private int _maxN;

    [ObservableProperty]
    private int _runsPerPoint;

    [ObservableProperty]
    private bool _showTheory;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isRunning;

    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private string _statusText = "Выберите параметры и нажмите «Запустить замер».";

    [ObservableProperty]
    private string _estimateText = string.Empty;

    [ObservableProperty]
    private bool _showTree = true;

    [ObservableProperty]
    private int _treeN;

    [ObservableProperty]
    private string _treeInfoText = string.Empty;

    public BenchmarkSettingsViewModel(
        BenchmarkRegistry registry,
        BenchmarkRunner runner,
        BenchmarkChartViewModel chart,
        AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(runner);
        ArgumentNullException.ThrowIfNull(chart);
        ArgumentNullException.ThrowIfNull(settings);

        _registry = registry;
        _runner = runner;
        _chart = chart;

        _selectedAlgorithm = registry.Find(settings.BenchmarkAlgorithm) ?? registry.Default;

        var metadata = _selectedAlgorithm.Metadata;
        _maxN = Math.Clamp(settings.BenchmarkMaxN, metadata.MinN, metadata.MaxN);
        _runsPerPoint = Math.Clamp(settings.BenchmarkRuns, 1, ExperimentConfig.MaxRuns);
        _showTheory = settings.ShowTheory;
        _treeN = (_selectedAlgorithm as ITraceableBenchmark)?.TraceDefaultN ?? 1;

        UpdateEstimate();
        UpdateTree();
    }

    public IReadOnlyList<IAlgorithmBenchmark> Algorithms => _registry.Algorithms;

    public ObservableCollection<BenchmarkRow> Rows { get; } = [];

    public int MinAllowedN => SelectedAlgorithm.Metadata.MinN;

    public int MaxAllowedN => SelectedAlgorithm.Metadata.MaxN;

    public int MaxRuns => ExperimentConfig.MaxRuns;

    public string ComplexityText => $"Сложность: {SelectedAlgorithm.Metadata.Complexity}";

    public string DescriptionText => SelectedAlgorithm.Metadata.Description;

    /// <summary>Есть ли для выбранного алгоритма дерево рекурсии.</summary>
    public bool IsTraceable => SelectedAlgorithm is ITraceableBenchmark;

    /// <summary>Наибольшее n для дерева рекурсии.</summary>
    public int TraceMaxN => (SelectedAlgorithm as ITraceableBenchmark)?.TraceMaxN ?? 1;

    /// <inheritdoc />
    public void Dispose()
    {
        _cts?.Cancel();
    }

    partial void OnSelectedAlgorithmChanged(IAlgorithmBenchmark value)
    {
        var metadata = value.Metadata;

        MaxN = Math.Clamp(MaxN, metadata.MinN, metadata.MaxN);
        Rows.Clear();
        ProgressValue = 0;
        StatusText = "Выберите параметры и нажмите «Запустить замер».";

        OnPropertyChanged(nameof(MinAllowedN));
        OnPropertyChanged(nameof(MaxAllowedN));
        OnPropertyChanged(nameof(ComplexityText));
        OnPropertyChanged(nameof(DescriptionText));
        OnPropertyChanged(nameof(IsTraceable));
        OnPropertyChanged(nameof(TraceMaxN));

        TreeN = (value as ITraceableBenchmark)?.TraceDefaultN ?? 1;

        _chart.Clear(metadata);
        UpdateEstimate();
        UpdateTree();
    }

    partial void OnMaxNChanged(int value) => UpdateEstimate();

    partial void OnRunsPerPointChanged(int value) => UpdateEstimate();

    partial void OnShowTheoryChanged(bool value) => _chart.SetTheoryVisible(value);

    partial void OnTreeNChanged(int value) => UpdateTree();

    partial void OnShowTreeChanged(bool value) => _chart.Tree.IsVisible = value;

    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task RunAsync()
    {
        var algorithm = SelectedAlgorithm;
        var config = new ExperimentConfig(MaxN, RunsPerPoint);

        using var cts = new CancellationTokenSource();
        _cts = cts;
        var token = cts.Token;

        var collected = new List<BenchmarkPoint>();

        Rows.Clear();
        ProgressValue = 0;
        StatusText = "Идёт замер…";
        IsRunning = true;

        // Progress<T> создан в UI-потоке, поэтому обработчик тоже выполняется в нём:
        // здесь безопасно трогать график и коллекции.
        var progress = new Progress<BenchmarkProgress>(report =>
        {
            if (!IsRunning)
            {
                return;
            }

            collected.Add(report.Point);
            Rows.Add(ToRow(report.Point));
            ProgressValue = report.Fraction * 100;
            StatusText = string.Create(
                CultureInfo.CurrentCulture,
                $"Готово точек: {report.CompletedPoints} из {report.TotalPoints}");

            _chart.Show(new BenchmarkSeries(algorithm.Metadata, collected.ToArray()), ShowTheory);
        });

        try
        {
            var series = await Task.Run(() => _runner.Run(algorithm, config, progress, token), token);

            _chart.Show(series, ShowTheory);
            ProgressValue = 100;
            StatusText = "Замер завершён.";
        }
        catch (OperationCanceledException)
        {
            StatusText = "Замер отменён.";
        }
        catch (ArgumentOutOfRangeException exception)
        {
            StatusText = exception.Message;
        }
        finally
        {
            _cts = null;
            IsRunning = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel() => _cts?.Cancel();

    private bool CanRun() => !IsRunning;

    private bool CanCancel() => IsRunning;

    /// <summary>
    /// Строит дерево рекурсии для выбранного алгоритма и n и передаёт его на график.
    /// Дерево небольшое (до нескольких сотен узлов), поэтому строится прямо в UI-потоке.
    /// </summary>
    private void UpdateTree()
    {
        if (SelectedAlgorithm is ITraceableBenchmark traceable)
        {
            var tree = traceable.BuildTrace(Math.Clamp(TreeN, 1, traceable.TraceMaxN));

            _chart.Tree.Show(tree);
            TreeInfoText = string.Create(
                CultureInfo.CurrentCulture,
                $"Узлов в дереве: {tree.NodeCount}")
                + (tree.IsTruncated ? " (показана часть)" : string.Empty);
        }
        else
        {
            _chart.Tree.Show(null);
            TreeInfoText = "Для этого алгоритма дерево рекурсии недоступно.";
        }
    }

    private void UpdateEstimate()
    {
        var duration = BenchmarkEstimator.EstimateDuration(
            SelectedAlgorithm.Metadata,
            new ExperimentConfig(MaxN, RunsPerPoint));

        var text = $"Оценка длительности: {FormatDuration(duration)}";

        if (duration.TotalMinutes >= 10)
        {
            text += ". Это очень долго — замер можно прервать кнопкой «Отмена».";
        }

        EstimateText = text;
    }

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalSeconds < 1)
        {
            return "меньше секунды";
        }

        if (duration.TotalSeconds < 60)
        {
            return string.Create(CultureInfo.CurrentCulture, $"≈ {duration.TotalSeconds:F0} с");
        }

        if (duration.TotalMinutes < 60)
        {
            return string.Create(CultureInfo.CurrentCulture, $"≈ {duration.TotalMinutes:F0} мин");
        }

        if (duration.TotalHours < 48)
        {
            return string.Create(CultureInfo.CurrentCulture, $"≈ {duration.TotalHours:F1} ч");
        }

        return string.Create(CultureInfo.CurrentCulture, $"≈ {duration.TotalDays:F1} сут.");
    }

    private static BenchmarkRow ToRow(BenchmarkPoint point) => new(
        point.N,
        FormatMs(point.AverageMs),
        FormatMs(point.MinMs),
        FormatMs(point.MaxMs));

    private static string FormatMs(double milliseconds)
    {
        var format = milliseconds >= 100 ? "F1" : milliseconds >= 1 ? "F3" : "F5";
        return milliseconds.ToString(format, CultureInfo.CurrentCulture);
    }
}
