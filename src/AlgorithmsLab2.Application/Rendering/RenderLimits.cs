namespace AlgorithmsLab2.Application.Rendering;

/// <summary>
/// Ограничения на параметры визуализации. Единый источник правды и для ползунков в UI,
/// и для валидации в рендерере, и для тестов.
/// </summary>
public static class RenderLimits
{
    public const int MinDepth = 0;

    /// <summary>
    /// Выше 7 смысла нет: растр ограничен 2187 = 3^7 пикселями, поэтому восьмой уровень
    /// всё равно мельче пикселя и только расходится с теоретической площадью.
    /// </summary>
    public const int MaxDepth = 7;

    public const int MinHanoiDisks = 1;

    public const int MaxHanoiDisks = 10;

    /// <summary>Минимальная сторона растра: мельче просто нет смысла.</summary>
    public const int MinPixelSide = 243;

    /// <summary>Максимальная сторона растра: 2187² ≈ 4,8 МБ, дальше страдает память.</summary>
    public const int MaxPixelSide = 2187;

    /// <summary>Разрешение растра для глубины: 3^depth, но не выходя за разумные границы.</summary>
    public static int PixelSideFor(int depth)
    {
        var natural = (int)Math.Pow(3, Math.Clamp(depth, 0, 12));
        return Math.Clamp(natural, MinPixelSide, MaxPixelSide);
    }

    public static int ClampDepth(int depth) => Math.Clamp(depth, MinDepth, MaxDepth);
}
