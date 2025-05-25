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

    public void AddOrderToRestaurantOrderList(Restaurant restaurant, Order order)
    {
        restaurant.AddOrderToList(order);
    }

    public List<Order> GetRestaurantOrdersInCooking(Restaurant restaurant)
    {
        return restaurant.GetAllOrdersThatAreCooking();
    }

    public List<Rating> GetRestaurantReviews(Restaurant restaurant)
    {
        return restaurant.Ratings;
    }

    public List<Order> GetAllorderThatAreReadyForDelivery(Restaurant restaurant)
    {
        return restaurant.GetAllorderThatAreReadyForDelivery(restaurant);
    }

    public void UpdateRestaurantOrderStatus(Restaurant restaurant, Order order)
    {
        restaurant.UpdateOrderStatus(order);
    }

    public List<RestaurantMenuItem> GetRestaurantMenuItems(string restaurantId)
    {
        var restaurant = GetRestaurantById(restaurantId);
        return restaurant.MenuItems;
    }
}