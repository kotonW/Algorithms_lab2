namespace AlgorithmsLab2.Application.Painting;

/// <summary>Цветовая схема фрактала: фон и цвет клеток.</summary>
public sealed record FractalPalette(string Name, Rgb Background, Rgb Foreground);

/// <summary>Набор доступных палитр (пункт F11).</summary>
public static class FractalPalettes
{
    public static IReadOnlyList<FractalPalette> All { get; } =
    [
        new("Графит", Rgb.FromHex("#14161B"), Rgb.FromHex("#E8ECF5")),
        new("Негатив", Rgb.FromHex("#F4F6FA"), Rgb.FromHex("#1B1F27")),
        new("Янтарь", Rgb.FromHex("#1A1206"), Rgb.FromHex("#FFB347")),
        new("Океан", Rgb.FromHex("#06121E"), Rgb.FromHex("#4FC3F7")),
        new("Изумруд", Rgb.FromHex("#06170F"), Rgb.FromHex("#4ADE80")),
        new("Пурпур", Rgb.FromHex("#160A1E"), Rgb.FromHex("#C084FC")),
    ];

    public static FractalPalette Default => All[0];

    /// <summary>Ищет палитру по имени, при неудаче возвращает палитру по умолчанию.</summary>
    public static FractalPalette Find(string? name) =>
        All.FirstOrDefault(palette => string.Equals(palette.Name, name, StringComparison.Ordinal)) ?? Default;
}
