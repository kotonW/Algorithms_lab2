using Avalonia.Controls;

namespace AlgorithmsLab2.App.Views;

public partial class HanoiSettingsView : UserControl
{
    public HanoiSettingsView() => InitializeComponent();

    private void OnLogRequestBringIntoView(object? sender, RequestBringIntoViewEventArgs e)
    {
        // Не даём событию всплыть к главному ScrollViewer окна:
        // внутри самого списка автоскролл сработает, а внешняя панель останется на месте.
        e.Handled = true;
    }
}