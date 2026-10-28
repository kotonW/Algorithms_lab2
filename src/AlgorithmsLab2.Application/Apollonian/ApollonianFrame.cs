using AlgorithmsLab2.Domain.Apollonian;

namespace AlgorithmsLab2.Application.Apollonian;

/// <summary>Один кадр для отрисовки: решётка, палитра и сколько уровней показать.</summary>
public sealed record ApollonianFrame(GasketResult Gasket, GasketPalette Palette, int VisibleLevels)
{
    /// <summary>Статистика по показанным уровням.</summary>
    public GasketStatistics Statistics => GasketStatistics.For(Gasket, VisibleLevels);
}
