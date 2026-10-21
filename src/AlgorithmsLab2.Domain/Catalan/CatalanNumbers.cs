using System.Numerics;

namespace AlgorithmsLab2.Domain.Catalan;

/// <summary>
/// Числа Каталана: 1, 1, 2, 5, 14, 42, … Они считают, например, бинарные деревья с n узлами
/// и способы расставить скобки в произведении из n + 1 множителя.
/// </summary>
/// <remarks>
/// Наивная рекурсия: C(0) = 1, C(n) = Σ C(i)·C(n − 1 − i), i = 0..n−1. Подзадачи не запоминаются,
/// поэтому они пересчитываются снова и снова. Для числа вызовов T(n) = 1 + 2·Σ T(i) получается
/// T(n) = 3·T(n − 1), то есть ровно 3^n: время растёт экспоненциально, хотя значение C(n) ≈ 4^n / n^1,5.
/// </remarks>
public static class CatalanNumbers
{
    /// <summary>Наибольшее n, для которого C(n) помещается в <see cref="long"/> (C(35) ≈ 3,1·10^18).</summary>
    public const int MaxSupportedN = 35;

    // Как часто рекурсия проверяет токен отмены (раз в 2^14 вызовов).
    private const long CancellationCheckMask = (1L << 14) - 1;

    /// <summary>Считает C(n) наивной рекурсией без запоминания и возвращает также число вызовов.</summary>
    /// <exception cref="OperationCanceledException">Если вычисление отменено.</exception>
    public static CatalanResult ComputeRecursive(int n, CancellationToken cancellationToken = default)
    {
        ValidateN(n);
        cancellationToken.ThrowIfCancellationRequested();

        var walker = new Walker(cancellationToken);
        var value = walker.Catalan(n);

        return new CatalanResult(value, walker.Calls);
    }

    /// <summary>Эталон для проверки: C(i + 1) = C(i) · 2(2i + 1) / (i + 2), за O(n).</summary>
    public static long Iterative(int n)
    {
        ValidateN(n);

        BigInteger value = 1;

        for (var i = 0; i < n; i++)
        {
            value = value * (2 * ((2 * i) + 1)) / (i + 2);
        }

        return (long)value;
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

    private sealed class Walker(CancellationToken cancellationToken)
    {
        private long _calls;

        public long Calls => _calls;

        public long Catalan(int n)
        {
            _calls++;

            if ((_calls & CancellationCheckMask) == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (n == 0)
            {
                return 1;
            }

            long sum = 0;

            for (var i = 0; i < n; i++)
            {
                sum += Catalan(i) * Catalan(n - 1 - i);
            }

            return sum;
        }
    }
}
