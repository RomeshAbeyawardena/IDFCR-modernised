namespace IDFCR.Utilities.Extensions;

/// <summary>
/// Defines 
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Determines whether the current enum flag shares any overlapping bits with another flag value.
    /// </summary>
    public static bool Overlaps<TEnum>(this TEnum value, TEnum other)
        where TEnum : Enum
    {
        long left = Convert.ToInt64(value);
        long right = Convert.ToInt64(other);

        // If either side is 0 (None), they only overlap if they are exactly identical
        if (left == 0 || right == 0)
        {
            return left == right;
        }

        return (left & right) != 0;
    }
}