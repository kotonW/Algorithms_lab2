namespace AlgorithmsLab2.Application.Settings;

/// <summary>Хранилище настроек. Реализация — в слое приложения Avalonia.</summary>
public interface ISettingsStore
{
    /// <summary>Читает настройки; при любой проблеме возвращает значения по умолчанию.</summary>
    AppSettings Load();

    void Save(AppSettings settings);
}
