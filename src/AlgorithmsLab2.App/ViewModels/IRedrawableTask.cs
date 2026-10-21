namespace AlgorithmsLab2.App.ViewModels;

/// <summary>
/// Задача, график которой хранит цвета внутри объекта Plot и должен перерисоваться при смене темы.
/// </summary>
public interface IRedrawableTask
{
    /// <summary>Перерисовывает график с учётом текущей темы.</summary>
    void Redraw();
}
