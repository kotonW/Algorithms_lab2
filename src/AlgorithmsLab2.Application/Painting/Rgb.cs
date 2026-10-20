using System.Globalization;

namespace AlgorithmsLab2.Application.Painting;

/// <summary>Цвет в формате RGB без привязки к UI-библиотеке.</summary>
public readonly record struct Rgb(byte R, byte G, byte B)
{
    public static Rgb FromHex(string hex)
    {
        var value = hex.TrimStart('#');

        if (value.Length != 6)
        {
            throw new FormatException($"Ожидался цвет вида #RRGGBB, получено «{hex}».");
        }

        return new Rgb(
            byte.Parse(value[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(value[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(value[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture));
    }
}
