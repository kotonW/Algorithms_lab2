using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Domain.Queens;

/// <summary>
/// Рекурсивный алгоритм поиска всех расстановок N не атакующих друг друга ферзей
/// на доске N×N (метод поиска с возвратом / бэктрекинг).
/// </summary>
public static class NQueensCounter
{
    /// <summary>Максимальный размер доски, поддерживаемый алгоритмом.</summary>
    public const int MaxSupportedN = 16;

    private const long CancellationCheckMask = (1L << 14) - 1;

    /// <summary>
    /// Находит все расстановки N ферзей и возвращает число решений и число рекурсивных вызовов.
    /// </summary>
    public static QueensResult Count(int n, CancellationToken cancellationToken = default)
    {
        if (n is < 1 or > MaxSupportedN)
        {
            throw new ArgumentOutOfRangeException(
                nameof(n), n, $"n должно быть от 1 до {MaxSupportedN}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var walker = new Walker(n, cancellationToken);
        walker.Run();

        return new QueensResult(walker.Solutions, walker.Calls);
    }

    /// <summary>
    /// Строит дерево вызовов рекурсии для небольшого n.
    /// Узел — попытка поставить ферзя в клетку (row, col).
    /// </summary>
    public static RecursionTree Trace(int n, int maxNodes = 400)
    {
        if (n is < 1 or > MaxSupportedN)
        {
            throw new ArgumentOutOfRangeException(
                nameof(n), n, $"n должно быть от 1 до {MaxSupportedN}.");
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(maxNodes, 1);

        var tracer = new Tracer(n, maxNodes);
        var root = tracer.Run();

        return new RecursionTree($"n = {n}", root, tracer.Count, tracer.Truncated);
    }

    private sealed class Walker(int n, CancellationToken cancellationToken)
    {
        private readonly int _allMask = (1 << n) - 1;

        public long Solutions { get; private set; }

        public long Calls { get; private set; }

        public void Run() => Place(0, 0, 0, 0);

        private void Place(int row, int cols, int diag1, int diag2)
        {
            Calls++;

            if ((Calls & CancellationCheckMask) == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (row == n)
            {
                Solutions++;
                return;
            }

            var available = _allMask & ~(cols | diag1 | diag2);

            while (available != 0)
            {
                var bit = available & -available;
                available &= available - 1;

                Place(row + 1, cols | bit, (diag1 | bit) << 1, (diag2 | bit) >> 1);
            }
        }
    }

    private sealed class Tracer(int n, int maxNodes)
    {
        private readonly int _allMask = (1 << n) - 1;
        private int _count = 1; // Учитываем корень "∅"

        public int Count => _count;

        public bool Truncated { get; private set; }

        public RecursionNode Run()
        {
            var root = new RecursionNode(0, "∅");
            Place(0, 0, 0, 0, root);
            return root;
        }

        private void Place(int row, int cols, int diag1, int diag2, RecursionNode parent)
        {
            if (row == n)
            {
                return;
            }

            var available = _allMask & ~(cols | diag1 | diag2);

            while (available != 0 && !Truncated)
            {
                if (_count >= maxNodes)
                {
                    Truncated = true;
                    return;
                }

                var bit = available & -available;
                available &= available - 1;

                var col = int.TrailingZeroCount(bit);
                var isSolution = row + 1 == n;
                var label = isSolution ? $"Q{row + 1}:{col + 1} ✔" : $"Q{row + 1}:{col + 1}";

                var node = new RecursionNode(_count++, label);
                parent.AddChild(node);

                Place(row + 1, cols | bit, (diag1 | bit) << 1, (diag2 | bit) >> 1, node);
            }
        }
    }
}