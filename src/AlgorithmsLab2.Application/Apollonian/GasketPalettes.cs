namespace AlgorithmsLab2.Application.Apollonian;

/// <summary>Набор готовых палитр.</summary>
public static class GasketPalettes
{
    /// <summary>Все палитры; первая — палитра по умолчанию.</summary>
    public static IReadOnlyList<GasketPalette> All { get; } =
    [
        new GasketPalette("Графит", new GasketRgb(70, 76, 92), new GasketRgb(150, 158, 175), new GasketRgb(235, 238, 245)),
        new GasketPalette("Океан", new GasketRgb(16, 52, 120), new GasketRgb(30, 144, 220), new GasketRgb(160, 230, 240)),
        new GasketPalette("Закат", new GasketRgb(88, 28, 110), new GasketRgb(230, 90, 70), new GasketRgb(255, 214, 102)),
        new GasketPalette("Лес", new GasketRgb(22, 70, 48), new GasketRgb(60, 160, 90), new GasketRgb(200, 235, 140)),
    ];

    /// <summary>Палитра по умолчанию.</summary>
    public static GasketPalette Default => All[0];

    /// <summary>Палитра по названию; если такой нет — палитра по умолчанию.</summary>
    public static GasketPalette Find(string? name) =>
        All.FirstOrDefault(palette => string.Equals(palette.Name, name, StringComparison.Ordinal)) ?? Default;
}
