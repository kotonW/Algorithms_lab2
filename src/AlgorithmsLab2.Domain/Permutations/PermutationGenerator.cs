using AlgorithmsLab2.Domain.Benchmarks;

namespace AlgorithmsLab2.Domain.Permutations;

/// <summary>
/// Рекурсивный перебор всех перестановок чисел 1..n (перебор с возвратом).
/// Элемент фиксируется на текущей позиции, хвост перебирается рекурсивно; всего n! результатов.
/// </summary>
public static class PermutationGenerator
{
    /// <summary>Наибольшее n, для которого n! помещается в <see cref="long"/> (20! ≈ 2,4·10^18).</summary>
    public const int MaxSupportedN = 20;

    // Как часто рекурсия проверяет токен отмены (раз в 2^14 вызовов): проверка дешёвая,
    // но на горячем пути её лучше не делать на каждом шаге.
    private const int CancellationCheckMask = (1 << 14) - 1;

    /// <summary>Факториал n.</summary>
    public static long Factorial(int n)
    {
        ValidateN(n);

        long result = 1;

        for (var i = 2; i <= n; i++)
        {
            result *= i;
        }

        return result;
    }

    /// <summary>
    /// Перебирает все перестановки и возвращает их число.
    /// </summary>
    /// <param name="n">Длина перестановки, 0..<see cref="MaxSupportedN"/>.</param>
    /// <param name="visitor">
    /// Вызывается для каждой перестановки. Массив переиспользуется между вызовами:
    /// чтобы сохранить результат, его нужно скопировать. Для замеров передают <c>null</c>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены, проверяется периодически.</param>
    public static long Enumerate(
        int n,
        Action<int[]>? visitor = null,
        CancellationToken cancellationToken = default)
    {
        ValidateN(n);
        cancellationToken.ThrowIfCancellationRequested();

        var items = new int[n];

        for (var i = 0; i < n; i++)
        {
            items[i] = i + 1;
        }

        var walker = new Walker(items, visitor, cancellationToken);
        walker.Permute(0);

        return walker.Count;
    }

    /// <summary>Возвращает все перестановки списком. Предназначено для небольших n (тесты, отладка).</summary>
    public static IReadOnlyList<int[]> GenerateAll(int n)
    {
        var result = new List<int[]>();
        Enumerate(n, permutation => result.Add((int[])permutation.Clone()));
        return result;
    }

    /// <summary>
    /// Строит дерево вызовов рекурсии для небольшого n: каждый узел — один вызов, подпись узла — уже
    /// зафиксированное начало перестановки, листья — готовые перестановки в порядке их получения.
    /// Это отдельный проход: он повторяет перебор, но не участвует в замерах времени.
    /// </summary>
    /// <param name="n">Длина перестановки.</param>
    /// <param name="maxNodes">Предел числа узлов: дерево n! листьев быстро становится огромным.</param>
    public static RecursionTree Trace(int n, int maxNodes = 400)
    {
        ValidateN(n);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxNodes, 1);

        var items = new int[n];

        for (var i = 0; i < n; i++)
        {
            items[i] = i + 1;
        }

        var tracer = new Tracer(items, maxNodes);
        var root = tracer.Visit(0) ?? throw new InvalidOperationException("Корень дерева не построен.");

        return new RecursionTree($"n = {n}", root, tracer.Count, tracer.Truncated);
    }

    private static void ValidateN(int n)
    {
        if (n is < 0 or > MaxSupportedN)
        {
            throw new ArgumentOutOfRangeException(
                nameof(n),
                n,
                $"n должно быть от 0 до {MaxSupportedN}.");
        }
    }

    private sealed class Tracer(int[] items, int maxNodes)
    {
        private int _count;

        public int Count => _count;

        public bool Truncated { get; private set; }

        public RecursionNode? Visit(int position)
        {
            if (_count >= maxNodes)
            {
                Truncated = true;
                return null;
            }

            var node = new RecursionNode(_count++, Label(position));

            if (position == items.Length)
            {
                return node;
            }

            for (var i = position; i < items.Length; i++)
            {
                (items[position], items[i]) = (items[i], items[position]);
                var child = Visit(position + 1);
                (items[position], items[i]) = (items[i], items[position]);

                if (child is null)
                {
                    break;
                }

                node.AddChild(child);
            }

            return node;
        }

        private string Label(int position) => position == 0 ? "∅" : string.Concat(items.Take(position));
    }

    private sealed class Walker(int[] items, Action<int[]>? visitor, CancellationToken cancellationToken)
    {
        private long _ticks;

        public long Count { get; private set; }

        public void Permute(int position)
        {
            if ((++_ticks & CancellationCheckMask) == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (position == items.Length)
            {
                Count++;
                visitor?.Invoke(items);
                return;
            }

            for (var i = position; i < items.Length; i++)
            {
                (items[position], items[i]) = (items[i], items[position]);
                Permute(position + 1);
                (items[position], items[i]) = (items[i], items[position]);
            }
        }
    }
}
