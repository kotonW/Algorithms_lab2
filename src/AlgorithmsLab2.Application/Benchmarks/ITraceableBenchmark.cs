using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Application.Benchmarks;

/// <summary>Алгоритм, для которого можно показать дерево рекурсии на небольшом n.</summary>
public interface ITraceableBenchmark : IAlgorithmBenchmark
{
    /// <summary>Значение n по умолчанию: дерево небольшое и помещается на экране с подписями.</summary>
    int TraceDefaultN { get; }

    /// <summary>Наибольшее n, для которого дерево ещё имеет смысл рисовать.</summary>
    int TraceMaxN { get; }

    /// <summary>Строит дерево вызовов для заданного n (1..<see cref="TraceMaxN"/>).</summary>
    RecursionTree BuildTrace(int n);
}
