using System.Numerics;

namespace AlgorithmsLab2.Domain.Apollonian;

/// <summary>
/// Аполлониева решётка: в единичный круг вписаны три равных круга, и каждая оставшаяся криволинейная
/// «щель» рекурсивно заполняется кругом, касающимся трёх её сторон, после чего у нового круга возникают
/// ещё три щели.
/// </summary>
/// <remarks>
/// <para>
/// Радиус нового круга и положение его центра даёт теорема Декарта. Если четыре круга попарно касаются
/// и известны три из них (кривизны k = 1/r, внешняя окружность берётся с отрицательной кривизной),
/// то второй круг, касающийся тех же трёх, находится <b>линейно</b>, без корней:
/// k' = 2(k₁ + k₂ + k₃) − k₄ и k'·z' = 2(k₁z₁ + k₂z₂ + k₃z₃) − k₄·z₄, где z — центр круга как комплексное число.
/// Это формула отражения из комплексной теоремы Декарта.
/// </para>
/// <para>
/// Число кругов растёт как 3^n: на уровне j появляется 4·3^(j−1) кругов, всего 2·3^n + 1 до глубины n.
/// Круги не пересекаются, а их суммарная площадь стремится к площади внешнего круга: сама решётка —
/// фрактал нулевой площади с хаусдорфовой размерностью ≈ 1,3057.
/// </para>
/// </remarks>
public static class ApolloniusGasket
{
    /// <summary>Наибольшая глубина: при 8 уже 13 123 круга, при 9 было бы около 39 000.</summary>
    public const int MaxDepth = 9;

    /// <summary>Радиус внешней окружности: сцена нормирована так, что он равен 1.</summary>
    public const double OuterRadius = 1.0;

    /// <summary>Хаусдорфова размерность аполлониевой решётки (Макмаллен, 1998).</summary>
    public const double HausdorffDimension = 1.3056868;

    /// <summary>Сколько кругов (без внешней окружности) будет при заданной глубине: 2·3^n + 1.</summary>
    public static long CircleCount(int depth)
    {
        ValidateDepth(depth);

        long power = 1;

        for (var i = 0; i < depth; i++)
        {
            power *= 3;
        }

        return (2 * power) + 1;
    }

    /// <summary>Строит решётку до заданной глубины.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Если глубина вне 0..<see cref="MaxDepth"/>.</exception>
    public static GasketResult Build(int depth)
    {
        ValidateDepth(depth);

        var circles = new List<GasketCircle>((int)CircleCount(depth));

        // Три равных круга, касающихся друг друга и внешней окружности: кривизна k = 1 + 2/√3 (радиус ≈ 0,4641).
        var curvature = 1.0 + (2.0 / Math.Sqrt(3.0));
        var offset = OuterRadius - (1.0 / curvature);

        var outer = new Circle(-1.0 / OuterRadius, Complex.Zero);
        var first = new Circle(curvature, Complex.FromPolarCoordinates(offset, Math.PI / 2.0));
        var second = new Circle(curvature, Complex.FromPolarCoordinates(offset, 7.0 * Math.PI / 6.0));
        var third = new Circle(curvature, Complex.FromPolarCoordinates(offset, 11.0 * Math.PI / 6.0));

        circles.Add(ToGasketCircle(first, 0));
        circles.Add(ToGasketCircle(second, 0));
        circles.Add(ToGasketCircle(third, 0));

        // Четыре начальные щели: центральная между тремя кругами и три между внешней окружностью и парой кругов.
        Fill(circles, depth, first, second, third, outer, 1);
        Fill(circles, depth, outer, first, second, third, 1);
        Fill(circles, depth, outer, second, third, first, 1);
        Fill(circles, depth, outer, first, third, second, 1);

        return new GasketResult(depth, circles);
    }

    /// <summary>
    /// Заполняет щель, ограниченную кругами <paramref name="a"/>, <paramref name="b"/>, <paramref name="c"/>.
    /// Круг <paramref name="known"/> тоже касается всех трёх, но лежит по другую сторону от щели, поэтому
    /// новый круг — его «отражение».
    /// </summary>
    private static void Fill(
        List<GasketCircle> circles,
        int depth,
        Circle a,
        Circle b,
        Circle c,
        Circle known,
        int level)
    {
        if (level > depth)
        {
            return;
        }

        var curvature = (2.0 * (a.Curvature + b.Curvature + c.Curvature)) - known.Curvature;
        var weighted = (2.0 * ((a.Curvature * a.Center) + (b.Curvature * b.Center) + (c.Curvature * c.Center)))
                       - (known.Curvature * known.Center);

        var created = new Circle(curvature, weighted / curvature);
        circles.Add(ToGasketCircle(created, level));

        // Новый круг делит щель на три: с каждой парой исходных кругов, а «известным» там остаётся третий.
        Fill(circles, depth, a, b, created, c, level + 1);
        Fill(circles, depth, b, c, created, a, level + 1);
        Fill(circles, depth, a, c, created, b, level + 1);
    }

    private static GasketCircle ToGasketCircle(Circle circle, int level) =>
        new(circle.Center.Real, circle.Center.Imaginary, 1.0 / circle.Curvature, level);

    private static void ValidateDepth(int depth)
    {
        if (depth is < 0 or > MaxDepth)
        {
            throw new ArgumentOutOfRangeException(
                nameof(depth),
                depth,
                $"Глубина должна быть от 0 до {MaxDepth}.");
        }
    }

    /// <summary>Круг в терминах теоремы Декарта: знаковая кривизна и центр на комплексной плоскости.</summary>
    private readonly record struct Circle(double Curvature, Complex Center);
}
