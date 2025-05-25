using AribaEats.Models;

namespace AribaEats.Helper;

/// <summary>
/// Provides a collection of reusable helper functions to perform common operations.
/// </summary>
public static class ReusableFunctions
{
    /// <summary>
    /// Calculates the distance between two locations based on their coordinates.
    /// This method uses Manhattan Distance, summing the absolute differences of the coordinates.
    /// </summary>
    /// <param name="location1">A tuple representing the first location's coordinates (x, y).</param>
    /// <param name="location2">A tuple representing the second location's coordinates (x, y).</param>
    /// <returns>The calculated distance as an integer.</returns>
    public static int CalculateDistance(Tuple<int, int> location1, Tuple<int, int> location2)
    {
        int xDiff = Math.Abs(location1.Item1 - location2.Item1);
        int yDiff = Math.Abs(location1.Item2 - location2.Item2);
        return xDiff + yDiff;
    }
}