namespace AlgorithmsLab2.Domain.Apollonian;

/// <summary>Один круг аполлониевой решётки.</summary>
/// <param name="X">Абсцисса центра (внешняя окружность — единичная, с центром в начале координат).</param>
/// <param name="Y">Ордината центра.</param>
/// <param name="Radius">Радиус.</param>
/// <param name="Level">
/// Уровень рекурсии: 0 — три начальных круга, 1 — круги, вписанные в первые четыре «щели», и так далее.
/// </param>
public readonly record struct GasketCircle(double X, double Y, double Radius, int Level);
