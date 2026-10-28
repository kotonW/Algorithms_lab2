using AlgorithmsLab2.Application.Apollonian;
using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Application.Hanoi;
using AlgorithmsLab2.Application.Settings;
using AlgorithmsLab2.App.Services;
using AlgorithmsLab2.App.ViewModels;
using AlgorithmsLab2.App.Views;
using AlgorithmsLab2.Domain.Apollonian;
using AlgorithmsLab2.Domain.GrayCode;
using AlgorithmsLab2.Domain.Permutations;
using AlgorithmsLab2.Domain.Polyominoes;
using Microsoft.Extensions.DependencyInjection;
using AlgorithmsLab2.Domain.Queens;

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

        services.AddSingleton(provider =>
            new HanoiSession(provider.GetRequiredService<AppSettings>().DiskCount));

        // Первое задание: рекурсивный фрактал — аполлониева решётка (вместо ковра Серпинского).
        services.AddSingleton(provider =>
        {
            var settings = provider.GetRequiredService<AppSettings>();

            return new ApollonianSession(
                Math.Clamp(settings.Depth, 0, ApolloniusGasket.MaxDepth),
                GasketPalettes.Find(settings.Palette));
        });

        services.AddSingleton<ApollonianSettingsViewModel>();
        services.AddSingleton<ApollonianPlotViewModel>();
        services.AddSingleton<HanoiSettingsViewModel>();
        services.AddSingleton<HanoiPlotViewModel>();

        // Замеры алгоритмов: чтобы добавить новый алгоритм, достаточно реализовать
        // IAlgorithmBenchmark и добавить одну регистрацию ниже.
        services.AddSingleton<GrayCodeAlgorithm>();
        services.AddSingleton<IAlgorithmBenchmark, GrayCodeBenchmark>();
        services.AddSingleton<RedelmeierPolyominoAlgorithm>();
        services.AddSingleton<IAlgorithmBenchmark, PolyominoBenchmark>();
        services.AddSingleton<NQueensAlgorithm>();
        services.AddSingleton<IAlgorithmBenchmark, NQueensBenchmark>();
        services.AddSingleton<BenchmarkRegistry>();
        services.AddSingleton<BenchmarkRunner>();
        services.AddSingleton<BenchmarkChartViewModel>();
        services.AddSingleton<BenchmarkSettingsViewModel>();

        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<MainWindow>();

        return services;
    }
}
