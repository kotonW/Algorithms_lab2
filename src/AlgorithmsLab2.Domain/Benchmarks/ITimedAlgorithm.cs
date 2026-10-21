namespace AlgorithmsLab2.Domain.Benchmarks;

/// <summary>
/// Чистый алгоритм без знания о замерах и интерфейсе: принимает вход, возвращает результат.
/// </summary>
public interface ITimedAlgorithm<TInput, TResult>
{
    /// <summary>Название алгоритма для интерфейса.</summary>
    string Name { get; }

    /// <summary>Выполняет алгоритм один раз.</summary>
    TResult Execute(TInput input);
}

/// <summary>
/// Алгоритм, который умеет прерываться: рекурсия периодически проверяет токен отмены.
/// Нужен, чтобы замер можно было остановить даже на очень долгих входах.
/// </summary>
public interface ICancellableTimedAlgorithm<TInput, TResult> : ITimedAlgorithm<TInput, TResult>
{
    /// <summary>Выполняет алгоритм, бросая <see cref="OperationCanceledException"/> при отмене.</summary>
    TResult Execute(TInput input, CancellationToken cancellationToken);
}
