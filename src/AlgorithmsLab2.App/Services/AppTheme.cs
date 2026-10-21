namespace AlgorithmsLab2.App.Services;

/// <summary>
/// Текущий режим оформления. ThemeService обновляет его вместе с ThemeVariant,
/// а графики читают его, чтобы подобрать цвета.
/// </summary>
public static class AppTheme
{
    /// <summary>Включена ли тёмная тема.</summary>
    public static bool IsDark { get; private set; } = true;

    /// <summary>Запоминает выбранную тему.</summary>
    public static void Set(bool dark) => IsDark = dark;
}
