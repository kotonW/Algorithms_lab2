using AlgorithmsLab2.Application.Hanoi;
using AlgorithmsLab2.Application.Rendering;
using AlgorithmsLab2.Application.Settings;
using AlgorithmsLab2.App.Services;
using AlgorithmsLab2.App.ViewModels;
using AlgorithmsLab2.App.Views;
using AlgorithmsLab2.Domain.Fractals;
using Microsoft.Extensions.DependencyInjection;

namespace AlgorithmsLab2.App.Composition;

/// <summary>Единственное место, где собирается граф зависимостей приложения.</summary>
public static class ServiceConfiguration
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ISettingsStore>(_ => new JsonSettingsStore());
        services.AddSingleton(provider => provider.GetRequiredService<ISettingsStore>().Load());
        services.AddSingleton<IThemeService, ThemeService>();

        services.AddSingleton<SierpinskiCarpet>();
        services.AddSingleton<IFractalRenderer, FractalRenderer>();

        services.AddSingleton(provider =>
            new HanoiSession(provider.GetRequiredService<AppSettings>().DiskCount));

        services.AddSingleton<SierpinskiSettingsViewModel>();
        services.AddSingleton<SierpinskiPlotViewModel>();
        services.AddSingleton<HanoiSettingsViewModel>();
        services.AddSingleton<HanoiPlotViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<MainWindow>();

        return services;
    }
}
