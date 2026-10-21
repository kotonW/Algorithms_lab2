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
