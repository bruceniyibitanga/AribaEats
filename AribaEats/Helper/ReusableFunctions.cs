using AribaEats.Models;

namespace AribaEats.Helper;

public static class ReusableFunctions
{
    public static int CalculateDistance(Tuple<int,int> location1, Tuple<int, int> location2)
    {
        int xDiff = Math.Abs(location1.Item1 - location2.Item1);
        int yDiff = Math.Abs(location1.Item2 - location2.Item2);
        return xDiff + yDiff;
    }
}