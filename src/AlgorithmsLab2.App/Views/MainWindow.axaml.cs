using AlgorithmsLab2.App.ViewModels;
using Avalonia.Controls;

namespace AlgorithmsLab2.App.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.SaveSettings();
        }
    }
}
