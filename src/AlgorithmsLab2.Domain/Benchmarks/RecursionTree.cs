namespace AlgorithmsLab2.Domain.Benchmarks;

/// <summary>Один вызов рекурсии в дереве: порядковый номер обхода, подпись и вложенные вызовы.</summary>
public sealed class RecursionNode
{
    private readonly List<RecursionNode> _children = [];

    public RecursionNode(int order, string label)
    {
        Order = order;
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }

    /// <summary>Номер вызова в порядке выполнения: 0 — самый первый.</summary>
    public int Order { get; }

    /// <summary>Подпись узла (например, уже зафиксированная часть перестановки).</summary>
    public string Label { get; }

    /// <summary>Вызовы, сделанные из этого узла, в том порядке, в каком они выполнялись.</summary>
    public IReadOnlyList<RecursionNode> Children => _children;

    internal void AddChild(RecursionNode child) => _children.Add(child);
}

/// <summary>Дерево вызовов рекурсивного алгоритма для небольшого n: так алгоритм выполнялся шаг за шагом.</summary>
/// <param name="Title">Краткое описание, например «n = 3».</param>
/// <param name="Root">Корень — самый первый вызов.</param>
/// <param name="NodeCount">Сколько узлов построено.</param>
/// <param name="IsTruncated">Дерево обрезано, потому что достигнут предел числа узлов.</param>
public sealed record RecursionTree(string Title, RecursionNode Root, int NodeCount, bool IsTruncated);
