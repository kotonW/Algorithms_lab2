using System.Globalization;
using AlgorithmsLab2.Application.Painting;
using AlgorithmsLab2.Application.Rendering;
using AlgorithmsLab2.Application.Settings;
using AlgorithmsLab2.Domain.Fractals;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlgorithmsLab2.App.ViewModels;

/// <summary>Правая панель для ковра Серпинского: параметры построения и информация о кадре.</summary>
public partial class SierpinskiSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private int _depth;

    [ObservableProperty]
    private FractalPalette _selectedPalette;

    [ObservableProperty]
    private bool _isProgressive;

    [ObservableProperty]
    private string _cellsText = string.Empty;

    [ObservableProperty]
    private string _coverageText = string.Empty;

    [ObservableProperty]
    private string _removedText = string.Empty;

    [ObservableProperty]
    private string _resolutionText = string.Empty;

    [ObservableProperty]
    private string _rasterTimeText = "—";

    [ObservableProperty]
    private string _measuredFillText = "—";

    public SierpinskiSettingsViewModel(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Palettes = FractalPalettes.All;
        _selectedPalette = FractalPalettes.Find(settings.Palette);
        _isProgressive = settings.Progressive;
        _depth = RenderLimits.ClampDepth(settings.Depth);

        DimensionText = FractalMath.Dimension.ToString("F4", CultureInfo.CurrentCulture);
        UpdateStaticInfo();
    }

    /// <summary>Параметры изменились — нужно перерисовать фрактал.</summary>
    public event EventHandler? SettingsChanged;

    public IReadOnlyList<FractalPalette> Palettes { get; }

    public int MinDepth => RenderLimits.MinDepth;

    public int MaxDepth => RenderLimits.MaxDepth;

    public string DimensionText { get; }

    /// <summary>Обновляет строки, которые зависят от фактически построенного кадра.</summary>
    public void ApplyStatistics(RenderStatistics statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        RasterTimeText = $"{statistics.RasterTime.TotalMilliseconds:F1} мс";

        MeasuredFillText = statistics.MeasuredFillRatio.ToString("P2", CultureInfo.CurrentCulture);
    }

    partial void OnDepthChanged(int value)
    {
        UpdateStaticInfo();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnSelectedPaletteChanged(FractalPalette value) => SettingsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnIsProgressiveChanged(bool value) => SettingsChanged?.Invoke(this, EventArgs.Empty);

    private void UpdateStaticInfo()
    {
        var depth = RenderLimits.ClampDepth(Depth);

        CellsText = FractalMath.CellCount(depth).ToString("N0", CultureInfo.CurrentCulture);
        CoverageText = FractalMath.Coverage(depth).ToString("P2", CultureInfo.CurrentCulture);
        RemovedText = FractalMath.RemovedShare(depth).ToString("P2", CultureInfo.CurrentCulture);

        var side = RenderLimits.PixelSideFor(depth);
        ResolutionText = $"{side} × {side} px";
    }
}
