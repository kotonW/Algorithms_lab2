namespace AlgorithmsLab2.App.ViewModels;

/// <summary>Запись журнала ходов: номер хода и какое кольцо с какого стержня на какой перенесено.</summary>
/// <param name="Number">Порядковый номер хода, начиная с 1.</param>
/// <param name="Disk">Номер кольца (1 — самое маленькое).</param>
/// <param name="From">Стержень, с которого снято кольцо.</param>
/// <param name="To">Стержень, на который кольцо положено.</param>
public sealed record HanoiLogEntry(int Number, int Disk, string From, string To)
{
    /// <summary>Текст записи для списка.</summary>
    public string MoveText => $"Кольцо {Disk}: {From} → {To}";
}
