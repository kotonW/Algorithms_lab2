using System.Collections.ObjectModel;
using AlgorithmsLab2.Application.Settings;
using AlgorithmsLab2.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Корневая ViewModel: список задач, текущая задача и тема оформления.
/// Центральная и правая панели просто показывают содержимое выбранной задачи.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly ISettingsStore _settingsStore;
    private readonly IThemeService _themeService;
    private readonly ApollonianSettingsViewModel _apollonianSettings;
    private readonly HanoiSettingsViewModel _hanoiSettings;
    private readonly BenchmarkSettingsViewModel _benchmarkSettings;
    private readonly BenchmarkChartViewModel _benchmarkChart;

    [ObservableProperty]
    private ITaskViewModel _selectedTask;

    [ObservableProperty]
    private bool _isDarkTheme;

    public MainWindowViewModel(
        ISettingsStore settingsStore,
        IThemeService themeService,
        AppSettings settings,
        ApollonianPlotViewModel apollonianPlot,
        ApollonianSettingsViewModel apollonianSettings,
        HanoiPlotViewModel hanoiPlot,
        HanoiSettingsViewModel hanoiSettings,
        BenchmarkChartViewModel benchmarkChart,
        BenchmarkSettingsViewModel benchmarkSettings)
    {
        ArgumentNullException.ThrowIfNull(settingsStore);
        ArgumentNullException.ThrowIfNull(themeService);
        ArgumentNullException.ThrowIfNull(settings);

        _settingsStore = settingsStore;
        _themeService = themeService;
        _apollonianSettings = apollonianSettings;
        _hanoiSettings = hanoiSettings;
        _benchmarkSettings = benchmarkSettings;
        _benchmarkChart = benchmarkChart;

        Tasks =
        [
            new TaskViewModel(
                "apollonian",
                "Аполлониева решётка",
                "Рекурсивная упаковка кругов",
                apollonianPlot,
                apollonianSettings),
            new TaskViewModel(
                "hanoi",
                "Ханойские башни",
                "Рекурсивный перебор ходов",
                hanoiPlot,
                hanoiSettings),
            new TaskViewModel(
                "permutations",
                "Собственные алгоритмы",
                "Рекурсивные алгоритмы",
                benchmarkChart,
                benchmarkSettings),
        ];

        _selectedTask = Tasks.FirstOrDefault(task => task.Id == settings.Task) ?? Tasks[0];
        _isDarkTheme = settings.DarkTheme;
        _themeService.Apply(_isDarkTheme);
    }

    public ObservableCollection<ITaskViewModel> Tasks { get; }

    /// <summary>Собирает текущее состояние, чтобы сохранить его при закрытии (пункт F13).</summary>
    public AppSettings CaptureSettings() => new()
    {
        Task = SelectedTask.Id,
        Depth = _apollonianSettings.Depth,
        Palette = _apollonianSettings.SelectedPalette.Name,
        Progressive = _apollonianSettings.IsProgressive,
        DarkTheme = IsDarkTheme,
        DiskCount = _hanoiSettings.DiskCount,
        StepDelayMs = _hanoiSettings.StepDelayMs,
        BenchmarkAlgorithm = _benchmarkSettings.SelectedAlgorithm.Metadata.Id,
        BenchmarkMaxN = _benchmarkSettings.MaxN,
        BenchmarkRuns = _benchmarkSettings.RunsPerPoint,
        ShowTheory = _benchmarkSettings.ShowTheory,
    };

    /// <summary>Сохраняет настройки. Ошибки записи не должны ронять приложение.</summary>
    public void SaveSettings() => _settingsStore.Save(CaptureSettings());

    partial void OnIsDarkThemeChanged(bool value)
    {
        _themeService.Apply(value);

        // Графики хранят цвета в объекте Plot, поэтому их нужно перерисовать.
        foreach (var task in Tasks)
        {
            if (task.Plot is IRedrawableTask redrawable)
            {
                redrawable.Redraw();
            }
        }
    }

    [RelayCommand]
    private void ToggleTheme() => IsDarkTheme = !IsDarkTheme;
}
