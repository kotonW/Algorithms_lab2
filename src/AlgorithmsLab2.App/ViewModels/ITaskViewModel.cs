namespace AlgorithmsLab2.App.ViewModels;

/// <summary>Пункт левого меню: задача, её визуализация и её настройки.</summary>
public interface ITaskViewModel
{
    string Id { get; }

    string Title { get; }

    string Subtitle { get; }

    /// <summary>ViewModel для центральной панели (визуализация).</summary>
    object Plot { get; }

    /// <summary>ViewModel для правой панели (настройки и информация).</summary>
    object Settings { get; }
}
