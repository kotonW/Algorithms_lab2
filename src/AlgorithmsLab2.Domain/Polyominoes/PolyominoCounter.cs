namespace AlgorithmsLab2.Domain.Polyominoes;

/// <summary>
/// Подсчёт фиксированных полимино алгоритмом Редельмайера.
/// Полимино из n клеток — связная фигура, склеенная из n квадратов по сторонам (как фигуры тетриса при n = 4).
/// «Фиксированные» значит, что повороты и отражения считаются разными фигурами: 1, 2, 6, 19, 63, 216, 760, …
/// </summary>
/// <remarks>
/// <para>
/// Идея алгоритма: фигура растёт от начальной клетки. Это всегда самая нижняя клетка своего ряда
/// и самая левая в нём, поэтому каждая фигура строится ровно один раз, без поиска дубликатов.
/// Рекурсия хранит множество «непроверенных» клеток, соседних с уже взятыми: из него по очереди
/// выбирается клетка, которая либо добавляется в фигуру (тогда рекурсия идёт глубже), либо
/// навсегда исключается из этой ветки. Клетка, один раз попавшая в множество, помечается как увиденная
/// и при откате пометка снимается (возврат).
/// </para>
/// <para>
/// Каждый шаг рекурсии — это одна найденная фигура размера от 1 до n, поэтому время работы
/// пропорционально числу полимино и растёт как λ^n / n, где λ ≈ 4,06 (постоянная Клярнера).
/// Память — O(n²): по одному массиву на каждый уровень рекурсии.
/// </para>
/// </remarks>
public static class PolyominoCounter
{
    /// <summary>Наибольшее n, для которого результат помещается в <see cref="long"/> (число полимино n = 28 порядка 10^15).</summary>
    public const int MaxSupportedN = 28;

    // Как часто рекурсия проверяет токен отмены (раз в 2^14 шагов).
    private const long CancellationCheckMask = (1L << 14) - 1;

    /// <summary>Считает фиксированные полимино из <paramref name="n"/> клеток.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Если n вне диапазона 1..<see cref="MaxSupportedN"/>.</exception>
    /// <exception cref="OperationCanceledException">Если подсчёт отменён.</exception>
    public static PolyominoResult CountFixed(int n, CancellationToken cancellationToken = default)
    {
        if (n is < 1 or > MaxSupportedN)
        {
            throw new ArgumentOutOfRangeException(
                nameof(n),
                n,
                $"n должно быть от 1 до {MaxSupportedN}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var walker = new Walker(n, cancellationToken);
        walker.Run();

        return new PolyominoResult(walker.Found, walker.Calls);
    }

    /// <summary>
    /// Рекурсивный обход. Доска хранится одним линейным массивом: клетка (x, y) имеет номер y·ширина + x.
    /// Начальная клетка стоит в нулевом ряду посередине; допустимы только клетки с номером больше начального,
    /// то есть выше начальной или правее неё в том же ряду.
    /// </summary>
    private sealed class Walker
    {
        private readonly int _n;
        private readonly int _origin;
        private readonly int[] _deltas;
        private readonly bool[] _seen;
        private readonly int[][] _untried;
        private readonly CancellationToken _cancellationToken;

        private long _calls;
        private long _found;

        public Walker(int n, CancellationToken cancellationToken)
        {
            // Фигура из n клеток уходит от начальной не дальше чем на n − 1 клетку по горизонтали
            // и по вертикали, поэтому «обёртки» через край ряда не происходит.
            var width = (2 * n) + 1;

            _n = n;
            _origin = n;
            _deltas = [1, -1, width, -width];
            _seen = new bool[width * (n + 2)];
            _seen[_origin] = true;
            _cancellationToken = cancellationToken;

            // Каждая взятая клетка добавляет не более трёх новых соседей.
            _untried = new int[n][];

            for (var level = 0; level < n; level++)
            {
                _untried[level] = new int[(3 * n) + 3];
            }
        }

        public long Found => _found;

        public long Calls => _calls;

        public void Run()
        {
            _untried[0][0] = _origin;
            Place(0, 1);
        }

        /// <param name="level">Сколько клеток уже взято в фигуру.</param>
        /// <param name="count">Сколько клеток лежит в множестве непроверенных этого уровня.</param>
        private void Place(int level, int count)
        {
            var current = _untried[level];
            var isLast = level + 1 == _n;

            while (count > 0)
            {
                count--;
                var cell = current[count];

                _calls++;

                if ((_calls & CancellationCheckMask) == 0)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                }

                if (isLast)
                {
                    // Фигура собрана целиком: n клеток.
                    _found++;
                    continue;
                }

                // Для следующего уровня множество — это оставшиеся непроверенные клетки плюс новые соседи.
                var next = _untried[level + 1];
                Array.Copy(current, next, count);

                var size = count;

                foreach (var delta in _deltas)
                {
                    var neighbor = cell + delta;

                    if (neighbor > _origin && !_seen[neighbor])
                    {
                        _seen[neighbor] = true;
                        next[size++] = neighbor;
                    }
                }

                Place(level + 1, size);

                // Возврат: снимаем пометки с клеток, добавленных на этом шаге.
                for (var i = count; i < size; i++)
                {
                    _seen[next[i]] = false;
                }
            }
        }
    }
}
