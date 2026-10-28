using AlgorithmsLab2.Domain.Apollonian;

namespace AlgorithmsLab2.Application.Apollonian;

/// <summary>
/// Общее состояние задачи: построенная решётка, палитра и сколько уровней сейчас показано.
/// Панель настроек меняет его, центральная панель слушает событие и перерисовывается.
/// </summary>
public sealed class ApollonianSession
{
    private GasketResult _gasket;
    private GasketPalette _palette;
    private int _visibleLevels;

    public ApollonianSession(int depth, GasketPalette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);

        _gasket = ApolloniusGasket.Build(depth);
        _palette = palette;
        _visibleLevels = depth;
    }

    /// <summary>Вызывается при любом изменении, которое влияет на картинку.</summary>
    public event EventHandler<ApollonianFrame>? Changed;

    /// <summary>Глубина, до которой построена решётка.</summary>
    public int Depth => _gasket.Depth;

    /// <summary>Сколько уровней показано сейчас (0..<see cref="Depth"/>).</summary>
    public int VisibleLevels => _visibleLevels;

    /// <summary>Текущая палитра.</summary>
    public GasketPalette Palette => _palette;

    /// <summary>Текущий кадр.</summary>
    public ApollonianFrame Frame => new(_gasket, _palette, _visibleLevels);

    /// <summary>Перестраивает решётку на новую глубину.</summary>
    /// <param name="depth">Новая глубина.</param>
    /// <param name="showAllLevels">
    /// <c>true</c> — сразу показать все уровни; <c>false</c> — начать с нулевого (для пошагового построения).
    /// </param>
    public void SetDepth(int depth, bool showAllLevels = true)
    {
        if (depth == _gasket.Depth && showAllLevels && _visibleLevels == depth)
        {
            return;
        }

        if (depth != _gasket.Depth)
        {
            _gasket = ApolloniusGasket.Build(depth);
        }

        _visibleLevels = showAllLevels ? depth : 0;
        Raise();
    }

    /// <summary>Меняет число показанных уровней; значение ограничивается диапазоном 0..<see cref="Depth"/>.</summary>
    public void SetVisibleLevels(int levels)
    {
        var clamped = Math.Clamp(levels, 0, _gasket.Depth);

        if (clamped == _visibleLevels)
        {
            return;
        }

        _visibleLevels = clamped;
        Raise();
    }

    /// <summary>Меняет палитру.</summary>
    public void SetPalette(GasketPalette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);

        if (ReferenceEquals(palette, _palette))
        {
            return;
        }

        _palette = palette;
        Raise();
    }

    private void Raise() => Changed?.Invoke(this, Frame);
}
