namespace AlgorithmsLab2.Application.Settings;

/// <summary>Настройки, которые переживают перезапуск (пункт F13).</summary>
public sealed record AppSettings
{
    public string Task { get; init; } = "sierpinski";

    public int Depth { get; init; } = 5;

    public string Palette { get; init; } = "Графит";

    public bool DarkTheme { get; init; } = true;

    public bool Progressive { get; init; }

    public int DiskCount { get; init; } = 4;

    public int StepDelayMs { get; init; } = 150;
}
