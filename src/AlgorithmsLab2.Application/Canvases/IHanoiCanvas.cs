using AlgorithmsLab2.Domain.Hanoi;

namespace AlgorithmsLab2.Application.Canvases;

/// <summary>Куда рисовать ханойские башни.</summary>
public interface IHanoiCanvas
{
    void Show(HanoiSnapshot snapshot);
}
