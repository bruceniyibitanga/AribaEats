namespace AribaEats.Helper;

public static class SessionState
{
    // Key: customerId, Value: HashSet of restaurantIds they've visited
    private static readonly Dictionary<string, HashSet<string>> CustomerVisitedRestaurants = new();

    public static bool HasVisitedRestaurant(string customerId, string restaurantId)
    {
        return CustomerVisitedRestaurants.ContainsKey(customerId)
               && CustomerVisitedRestaurants[customerId].Contains(restaurantId);
    }

    public static void MarkRestaurantVisited(string customerId, string restaurantId)
    {
        if (!CustomerVisitedRestaurants.ContainsKey(customerId))
            CustomerVisitedRestaurants[customerId] = new HashSet<string>();

        CustomerVisitedRestaurants[customerId].Add(restaurantId);
    }
    
    public static void ClearVisitedRestaurants(string customerId)
    {
        SessionState.CustomerVisitedRestaurants.Remove(customerId);
    }
}
