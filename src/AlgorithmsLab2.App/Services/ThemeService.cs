using Avalonia.Styling;

namespace AlgorithmsLab2.App.Services;

/// <inheritdoc />
public sealed class ThemeService : IThemeService
{
    /// <inheritdoc />
    public void Apply(bool dark)
    {
        if (Avalonia.Application.Current is { } application)
        {
            application.RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }
}
