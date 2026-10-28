namespace AlgorithmsLab2.Application.Apollonian;

/// <summary>
/// Палитра решётки: плавный переход между опорными цветами. Круги крупных уровней окрашиваются
/// первым цветом, самые мелкие — последним.
/// </summary>
public sealed class GasketPalette
{
    private readonly GasketRgb[] _stops;

    public GasketPalette(string name, params GasketRgb[] stops)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(stops);

        if (stops.Length < 2)
        {
            throw new ArgumentException("Нужно как минимум два опорных цвета.", nameof(stops));
        }

        Name = name;
        _stops = stops;
    }

    /// <summary>Название палитры для списка.</summary>
    public string Name { get; }

    /// <summary>Цвет для положения <paramref name="t"/> от 0 (крупные круги) до 1 (самые мелкие).</summary>
    public GasketRgb ColorAt(double t)
    {
        var clamped = Math.Clamp(t, 0.0, 1.0);
        var scaled = clamped * (_stops.Length - 1);
        var index = Math.Min((int)scaled, _stops.Length - 2);
        var local = scaled - index;

        var lower = _stops[index];
        var upper = _stops[index + 1];

        return new GasketRgb(
            Mix(lower.R, upper.R, local),
            Mix(lower.G, upper.G, local),
            Mix(lower.B, upper.B, local));
    }

    private static byte Mix(byte start, byte end, double t) => (byte)Math.Round(start + ((end - start) * t));
}
