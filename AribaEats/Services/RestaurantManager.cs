using AribaEats.Models;

namespace AribaEats.Services;

public class RestaurantManager
{
    private List<Restaurant> _restaurants = new List<Restaurant>();

    public List<Restaurant> GetRestrauntList => _restaurants;

    public Restaurant GetRestaurantById(string restaurantId) => _restaurants.FirstOrDefault(x => x.Id == restaurantId);
}