using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Адаптер «любой алгоритм → бенчмарк»: из n строит вход и запускает алгоритм.</summary>
public class TimedAlgorithmBenchmark<TInput, TResult> : AlgorithmBenchmark
{
    private readonly ITimedAlgorithm<TInput, TResult> _algorithm;
    private readonly Func<int, TInput> _inputFactory;
    private readonly Func<TResult, long>? _operationsSelector;

    /// <param name="algorithm">Чистый алгоритм.</param>
    /// <param name="metadata">Описание алгоритма.</param>
    /// <param name="inputFactory">Как получить вход алгоритма из n.</param>
    /// <param name="operationsSelector">
    /// Как получить число операций из результата. Если не задано, используется результат типа long, иначе 0.
    /// </param>
    public TimedAlgorithmBenchmark(
        ITimedAlgorithm<TInput, TResult> algorithm,
        AlgorithmMetadata metadata,
        Func<int, TInput> inputFactory,
        Func<TResult, long>? operationsSelector = null)
        : base(metadata)
    {
        _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        _inputFactory = inputFactory ?? throw new ArgumentNullException(nameof(inputFactory));
        _operationsSelector = operationsSelector;
    }

    /// <inheritdoc />
    public override long RunOnce(int n, CancellationToken cancellationToken)
    {
        ValidateSize(n);

        var input = _inputFactory(n);

        var result = _algorithm is ICancellableTimedAlgorithm<TInput, TResult> cancellable
            ? cancellable.Execute(input, cancellationToken)
            : _algorithm.Execute(input);

        if (_operationsSelector is not null)
        {
            return _operationsSelector(result);
        }

        return result is long operations ? operations : 0;
    }
}
