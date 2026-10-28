namespace AlgorithmsLab2.Application.Settings;

/// <summary>Настройки, которые переживают перезапуск (пункт F13).</summary>
public sealed record AppSettings
{
    public string Task { get; init; } = "apollonian";

    public int Depth { get; init; } = 5;

    public string Palette { get; init; } = "Графит";

    public bool DarkTheme { get; init; } = true;

    public bool Progressive { get; init; }

    public int DiskCount { get; init; } = 4;

    public int StepDelayMs { get; init; } = 150;

    public string BenchmarkAlgorithm { get; init; } = "permutations";

    public int BenchmarkMaxN { get; init; } = 9;

    public int BenchmarkRuns { get; init; } = 5;

    public bool ShowTheory { get; init; } = true;
}
