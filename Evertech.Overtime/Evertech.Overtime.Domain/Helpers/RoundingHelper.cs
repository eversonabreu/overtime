namespace Evertech.Overtime.Domain.Helpers;

public static class RoundingHelper
{
    private const int DecimalPlaces = 2;

    public static decimal RoundMonetary(decimal value) =>
        Math.Floor(value * 100m) / 100m;

    public static decimal RoundHalfUp(decimal value) =>
        Math.Round(value, DecimalPlaces, MidpointRounding.AwayFromZero);
}