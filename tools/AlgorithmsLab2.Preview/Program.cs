using AlgorithmsLab2.Application.Benchmarks;
using AlgorithmsLab2.Application.Painting;
using AlgorithmsLab2.Application.Rendering;
using AlgorithmsLab2.Domain.Fractals;
using AlgorithmsLab2.Domain.Hanoi;
using AlgorithmsLab2.Domain.Permutations;
using AlgorithmsLab2.Rendering;

// Утилита разработчика: рендерит контрольные картинки без запуска UI,
// чтобы глазами проверить геометрию и цвета. Аргумент --bench печатает точки замеров.
if (args.Length > 0 && args[0] == "--bench")
{
    var permutations = new PermutationsBenchmark(new AllPermutationsAlgorithm());
    var measured = new BenchmarkRunner().Run(
        permutations,
        new ExperimentConfig(10, 3),
        null,
        CancellationToken.None);

    foreach (var point in measured.Points)
    {
        Console.WriteLine(
            $"{point.N}\t{point.AverageMs:F6}\t{point.Operations}\t{point.AverageMs / point.Operations:E3}");
    }

    return;
}

var outputDirectory = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "preview");

Directory.CreateDirectory(outputDirectory);

var renderer = new FractalRenderer(new SierpinskiCarpet());
var palette = FractalPalettes.Find(args.Length > 1 ? args[1] : "Графит");

foreach (var depth in new[] { 2, 4, 6, 8 })
{
    var frame = renderer.Render(SierpinskiRenderRequest.ForDepth(depth), CancellationToken.None);
    var plot = new ScottPlot.Plot();
    SierpinskiPlotRenderer.Draw(plot, frame.Raster, palette, 1.0);
    var path = Path.Combine(outputDirectory, $"carpet-{depth}.png");
    plot.SavePng(path, 640, 640);

    Console.WriteLine(
        $"ковёр depth={depth}: клеток={frame.Statistics.Cells}, "
        + $"пикселей={frame.Statistics.PixelSide}, "
        + $"доля={frame.Statistics.MeasuredFillRatio:P2}, "
        + $"теория={frame.Statistics.Coverage:P2}, "
        + $"{frame.Statistics.RasterTime.TotalMilliseconds:F1} мс -> {path}");
}

foreach (var diskCount in new[] { 3, 4, 10 })
{
    var puzzle = new HanoiPuzzle(diskCount);
    var states = new (int Moves, string Name)[]
    {
        (0, "start"),
        (puzzle.TotalMoves / 2, "middle"),
        (puzzle.TotalMoves, "final"),
    };

    foreach (var (moves, name) in states)
    {
        var snapshot = puzzle.Seek(moves);
        var plot = new ScottPlot.Plot();
        HanoiPlotRenderer.Draw(plot, snapshot, 640.0 / 480.0, dark: true);
        var path = Path.Combine(outputDirectory, $"hanoi-{diskCount}-{name}.png");
        plot.SavePng(path, 640, 480);
        Console.WriteLine($"башни {diskCount} колец, {name}: {snapshot.MovesMade}/{snapshot.TotalMoves} -> {path}");
    }
}

Console.WriteLine("готово");
