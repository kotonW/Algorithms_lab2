namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Одна задача в левом списке. Хранит готовые ViewModel для центральной панели
/// и для панели настроек, чтобы окно просто подставляло их через DataTemplate.
/// </summary>
public sealed class TaskViewModel(
    string id,
    string title,
    string subtitle,
    object plot,
    object settings) : ITaskViewModel
{
    /// <inheritdoc />
    public string Id { get; } = id;

    /// <inheritdoc />
    public string Title { get; } = title;

    /// <inheritdoc />
    public string Subtitle { get; } = subtitle;

    /// <inheritdoc />
    public object Plot { get; } = plot;

    /// <inheritdoc />
    public object Settings { get; } = settings;
}
