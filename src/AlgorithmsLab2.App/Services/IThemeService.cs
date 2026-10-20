namespace AlgorithmsLab2.App.Services;

/// <summary>Переключение светлой и тёмной темы оформления.</summary>
public interface IThemeService
{
    void Apply(bool dark);
}
