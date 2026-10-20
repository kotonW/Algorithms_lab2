using AlgorithmsLab2.App.Services;
using AlgorithmsLab2.App.ViewModels;
using AlgorithmsLab2.Rendering;
using Avalonia;
using Avalonia.Controls;
using ScottPlot.Avalonia;

namespace AlgorithmsLab2.App.Views;

public partial class SierpinskiPlotView : UserControl
{
    private SierpinskiPlotViewModel? _attached;

    public SierpinskiPlotView() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        Connect();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Connect();
    }

    private void Connect()
    {
        if (DataContext is not SierpinskiPlotViewModel viewModel || ReferenceEquals(_attached, viewModel))
        {
            return;
        }

        if (this.FindControl<AvaPlot>("PlotControl") is not { } plot)
        {
            return;
        }

        _attached = viewModel;
        viewModel.Attach(new AvaPlotCanvas(plot, () => AppTheme.IsDark));

        // Пропорции квадрата фрактала зависят от размера панели — перерисовываем при изменении.
        plot.SizeChanged += (_, _) => viewModel.RequestRedraw();
    }
}
