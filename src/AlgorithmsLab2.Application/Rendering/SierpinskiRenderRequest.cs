namespace AlgorithmsLab2.Application.Rendering;

/// <summary>Запрос на построение одного кадра ковра Серпинского.</summary>
public sealed record SierpinskiRenderRequest(int Depth, int PixelSide)
{
    public static SierpinskiRenderRequest ForDepth(int depth) =>
        new(RenderLimits.ClampDepth(depth), RenderLimits.PixelSideFor(depth));
}
