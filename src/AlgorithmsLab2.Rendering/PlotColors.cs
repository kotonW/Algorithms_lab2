namespace AlgorithmsLab2.Rendering;

/// <summary>Цвета графиков для светлой и тёмной темы: ScottPlot сам тему не подхватывает.</summary>
public sealed record PlotColors(
    ScottPlot.Color Background,
    ScottPlot.Color Text,
    ScottPlot.Color Grid,
    ScottPlot.Color Measured,
    ScottPlot.Color Theory,
    ScottPlot.Color LegendBackground,
    ScottPlot.Color LegendBorder,
    ScottPlot.Color Neutral,
    ScottPlot.Color NeutralDark,
    ScottPlot.Color Muted)
{
    public static readonly PlotColors Dark = new(
        Background: new ScottPlot.Color(30, 33, 40),
        Text: new ScottPlot.Color(215, 220, 230),
        Grid: new ScottPlot.Color(60, 65, 78),
        Measured: new ScottPlot.Color(90, 170, 255),
        Theory: new ScottPlot.Color(255, 160, 60),
        LegendBackground: new ScottPlot.Color(40, 44, 54),
        LegendBorder: new ScottPlot.Color(90, 96, 112),
        Neutral: new ScottPlot.Color(120, 128, 145),
        NeutralDark: new ScottPlot.Color(72, 78, 94),
        Muted: new ScottPlot.Color(139, 148, 166));

    public static readonly PlotColors Light = new(
        Background: new ScottPlot.Color(250, 250, 252),
        Text: new ScottPlot.Color(35, 38, 46),
        Grid: new ScottPlot.Color(220, 223, 230),
        Measured: new ScottPlot.Color(25, 110, 210),
        Theory: new ScottPlot.Color(225, 110, 10),
        LegendBackground: new ScottPlot.Color(255, 255, 255),
        LegendBorder: new ScottPlot.Color(190, 195, 205),
        Neutral: new ScottPlot.Color(140, 148, 162),
        NeutralDark: new ScottPlot.Color(175, 181, 195),
        Muted: new ScottPlot.Color(107, 116, 132));

    /// <summary>Набор цветов для текущей темы.</summary>
    public static PlotColors For(bool dark) => dark ? Dark : Light;
}
