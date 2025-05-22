using AribaEats.Helper;
using AribaEats.Models;

namespace AribaEats.Services;

public class RestaurantManager
{
    private List<Restaurant> _restaurants = new List<Restaurant>();

    public List<Restaurant> GetRestrauntList () => _restaurants;

    public Restaurant GetRestaurantById(string restaurantId) => _restaurants.FirstOrDefault(x => x.Id == restaurantId);

    public string AddMenuItem(Client client)
    {
        RestaurantMenuInputCollector collector = new RestaurantMenuInputCollector();
        var item = collector.CollectMenuInfo(client);
        if (!string.IsNullOrWhiteSpace(item.Name)) client.Restaurant.MenuItems.Add(item);
        else return "";
        return $"Successfully added {item.Name} (${item.Price}) to menu.";
    }
    public void AddNewRestaurantToList(Restaurant restaurant)
    {
        _restaurants.Add(restaurant);
    }

    public List<RestaurantMenuItem> GetRestaurantMenuItems(string restaurantId)
    {
        var restaurant = GetRestaurantById(restaurantId);
        return restaurant.MenuItems;
    }
}