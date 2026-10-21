using AlgorithmsLab2.App.Services;
using AlgorithmsLab2.App.ViewModels;
using AlgorithmsLab2.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ScottPlot.Avalonia;

namespace AlgorithmsLab2.App.Views;

public partial class BenchmarkChartView : UserControl
{
    /// <summary>Смещение мыши (в DIP), после которого нажатие считается перетаскиванием, а не кликом.</summary>
    private const double DragThreshold = 4;

    private BenchmarkChartViewModel? _attached;
    private AvaPlot? _plot;
    private Point? _pressPoint;
    private bool _dragged;

    public BenchmarkChartView() => InitializeComponent();

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
        if (DataContext is not BenchmarkChartViewModel viewModel || ReferenceEquals(_attached, viewModel))
        {
            return;
        }

        if (this.FindControl<AvaPlot>("PlotControl") is not { } plot)
        {
            return;
        }

        _attached = viewModel;

        if (!ReferenceEquals(_plot, plot))
        {
            _plot = plot;

            // Обработчики на туннельной стадии и с handledEventsToo: так они видят нажатия
            // раньше ScottPlot и не мешают ему панорамировать и масштабировать график.
            plot.AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
            plot.AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);
            plot.AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel, handledEventsToo: true);
            plot.PointerCaptureLost += OnPointerCaptureLost;
        }

        viewModel.Attach(new AvaPlotCanvas(plot, () => AppTheme.IsDark));
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_attached is null || _plot is null || !e.GetCurrentPoint(_plot).Properties.IsLeftButtonPressed)
        {
            return;
        }

        var position = e.GetPosition(_plot);
        _pressPoint = position;
        _dragged = false;

        _attached.PickPoint((float)position.X, (float)position.Y);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_pressPoint is not { } start || _dragged || _plot is null)
        {
            return;
        }

        // Пока кнопка зажата и мышь сдвинулась — это панорамирование, подпись мешала бы.
        var current = e.GetPosition(_plot);
        var dx = current.X - start.X;
        var dy = current.Y - start.Y;

        if (Math.Sqrt((dx * dx) + (dy * dy)) >= DragThreshold)
        {
            _dragged = true;
            _attached?.ReleasePoint();
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e) => EndPress();

    private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => EndPress();

    private void EndPress()
    {
        _pressPoint = null;
        _dragged = false;
        _attached?.ReleasePoint();
    }
}
