using AlgorithmsLab2.Application.Settings;
using AlgorithmsLab2.App.Composition;
using AlgorithmsLab2.App.Services;
using AlgorithmsLab2.App.ViewModels;
using AlgorithmsLab2.App.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;

namespace AlgorithmsLab2.App;

public partial class App : Avalonia.Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddAppServices();
        _serviceProvider = services.BuildServiceProvider();

        // Тему применяем до показа окна, чтобы не было мигания светлым фоном.
        var settings = _serviceProvider.GetRequiredService<AppSettings>();
        RequestedThemeVariant = settings.DarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = _serviceProvider.GetRequiredService<MainWindow>();
            window.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = window;

            desktop.ShutdownRequested += OnShutdownRequested;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        if (_serviceProvider?.GetService<MainWindowViewModel>() is { } viewModel)
        {
            viewModel.SaveSettings();
        }
    }
}
