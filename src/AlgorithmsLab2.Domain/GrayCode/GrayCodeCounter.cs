namespace AlgorithmsLab2.Domain.GrayCode;

/// <summary>
/// Рекурсивная генерация кода Грея (binary reflected Gray code).
/// <para>
/// Код Грея длины n — это перестановка всех 2^n двоичных строк длины n,
/// в которой любые две соседние строки (включая последнюю и первую) отличаются
/// ровно в одном бите.
/// </para>
/// <para>
/// Рекурсивное правило:
/// <c>G(0) = [ε]</c>,
/// <c>G(n) = 0·G(n−1) ++ 1·reverse(G(n−1))</c>.
/// </para>
/// <para>
/// Внутри используется обход «с флагом reverse»: вместо того чтобы явно строить
/// и разворачивать список, рекурсия идёт по дереву, в котором на каждом шаге
/// известен префикс и режим (прямой/обратный). Листья выдаются ровно в порядке
/// кода Грея. Сложность O(2^n) по времени, O(n) по памяти (стек рекурсии).
/// </para>
/// </summary>
public static class GrayCodeCounter
{
    /// <summary>
    /// Наибольшее n, для которого результат помещается в int при сдвиге <c>1 &lt;&lt; n</c>.
    /// Для подсчёта (без хранения последовательности) работает и больше,
    /// но здесь ограничено 28, чтобы <see cref="Generate"/> не съел всю память.
    /// </summary>
    public const int MaxSupportedN = 28;

    private const long CancellationCheckMask = (1L << 14) - 1;

    /// <summary>
    /// Считает все коды Грея длины n, не сохраняя их: возвращает количество,
    /// число шагов рекурсии и XOR-контрольную сумму.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Если n вне 0..<see cref="MaxSupportedN"/>.</exception>
    /// <exception cref="OperationCanceledException">Если операция отменена.</exception>
    public static GrayCodeResult Count(int n, CancellationToken cancellationToken = default)
    {
        if (n is < 0 or > MaxSupportedN)
        {
            throw new ArgumentOutOfRangeException(
                nameof(n),
                n,
                $"n должно быть от 0 до {MaxSupportedN}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var walker = new CountingWalker(cancellationToken);
        walker.Emit(n, 0, reversed: false);

        return new GrayCodeResult(walker.Count, walker.Calls, walker.Checksum);
    }

    /// <summary>
    /// Возвращает все коды Грея длины n в порядке генерации.
    /// Внимание: объём результата — O(2^n), для n &gt; 24 это уже сотни мегабайт.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Если n вне 0..<see cref="MaxSupportedN"/>.</exception>
    /// <exception cref="OperationCanceledException">Если операция отменена.</exception>
    public static List<int> Generate(int n, CancellationToken cancellationToken = default)
    {
        if (n is < 0 or > MaxSupportedN)
        {
            throw new ArgumentOutOfRangeException(
                nameof(n),
                n,
                $"n должно быть от 0 до {MaxSupportedN}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result = new List<int>(1 << n);
        var walker = new ListWalker(result, cancellationToken);
        walker.Emit(n, 0, reversed: false);
        return result;
    }

    /// <summary>Рекурсивный обход, который только считает (не хранит последовательность).</summary>
    private sealed class CountingWalker
    {
        private readonly CancellationToken _cancellationToken;

        public CountingWalker(CancellationToken cancellationToken) => _cancellationToken = cancellationToken;

        public long Count { get; private set; }

        public long Calls { get; private set; }

        public long Checksum { get; private set; }

        /// <param name="remaining">Сколько бит ещё нужно дописать.</param>
        /// <param name="prefix">Уже накопленный префикс (биты интерпретируются как целое).</param>
        /// <param name="reversed">
        /// true — идём по G(remaining) в обратном порядке; false — в прямом.
        /// </param>
        public void Emit(int remaining, int prefix, bool reversed)
        {
            Calls++;

            if ((Calls & CancellationCheckMask) == 0)
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }

            if (remaining == 0)
            {
                Count++;
                Checksum ^= prefix;
                return;
            }

            if (!reversed)
            {
                // прямой порядок: 0·G(n−1) ++ 1·reverse(G(n−1))
                Emit(remaining - 1, (prefix << 1) | 0, reversed: false);
                Emit(remaining - 1, (prefix << 1) | 1, reversed: true);
            }
            else
            {
                // обратный порядок: 1·G(n−1) ++ 0·reverse(G(n−1))
                Emit(remaining - 1, (prefix << 1) | 1, reversed: false);
                Emit(remaining - 1, (prefix << 1) | 0, reversed: true);
            }
        }
    }

    /// <summary>Рекурсивный обход, который складывает коды в список.</summary>
    private sealed class ListWalker
    {
        private readonly List<int> _output;
        private readonly CancellationToken _cancellationToken;
        private long _calls;

        public ListWalker(List<int> output, CancellationToken cancellationToken)
        {
            _output = output;
            _cancellationToken = cancellationToken;
        }

        public void Emit(int remaining, int prefix, bool reversed)
        {
            _calls++;

            if ((_calls & CancellationCheckMask) == 0)
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }

            if (remaining == 0)
            {
                _output.Add(prefix);
                return;
            }

            if (!reversed)
            {
                Emit(remaining - 1, (prefix << 1) | 0, reversed: false);
                Emit(remaining - 1, (prefix << 1) | 1, reversed: true);
            }
            else
            {
                Emit(remaining - 1, (prefix << 1) | 1, reversed: false);
                Emit(remaining - 1, (prefix << 1) | 0, reversed: true);
            }
        }
    }
}