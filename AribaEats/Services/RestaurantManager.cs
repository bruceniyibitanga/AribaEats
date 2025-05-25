using AribaEats.Helper;
using AribaEats.Models;

namespace AribaEats.Services;

/// <summary>
/// Handles operations related to managing restaurants, their menus, orders, and reviews.
/// </summary>
public class RestaurantManager
{
    private List<Restaurant> _restaurants = new List<Restaurant>(); // Internal list of restaurants managed by the system.

    /// <summary>
    /// Retrieves the full list of restaurants managed by the system.
    /// </summary>
    /// <returns>A list of <see cref="Restaurant"/> objects.</returns>
    public List<Restaurant> GetRestrauntList() => _restaurants;

    /// <summary>
    /// Finds and retrieves a restaurant by its unique identifier.
    /// </summary>
    /// <param name="restaurantId">The unique ID of the restaurant.</param>
    /// <returns>The <see cref="Restaurant"/> object if found, otherwise null.</returns>
    public Restaurant GetRestaurantById(string restaurantId) => _restaurants.FirstOrDefault(x => x.Id == restaurantId);

    /// <summary>
    /// Adds a new menu item to a client's restaurant menu.
    /// Collects menu information using the <see cref="RestaurantMenuInputCollector"/> helper.
    /// </summary>
    /// <param name="client">The client who owns the restaurant.</param>
    /// <returns>A success message if the menu item is added, otherwise an empty string.</returns>
    public string AddMenuItem(Client client)
    {
        RestaurantMenuInputCollector collector = new RestaurantMenuInputCollector();
        var item = collector.CollectMenuInfo(client);
        if (!string.IsNullOrWhiteSpace(item.Name)) 
            client.Restaurant.MenuItems.Add(item);
        else 
            return "";
        return $"Successfully added {item.Name} (${item.Price}) to menu.";
    }

    /// <summary>
    /// Adds a new restaurant to the internal restaurant list.
    /// </summary>
    /// <param name="restaurant">The <see cref="Restaurant"/> object to be added.</param>
    public void AddNewRestaurantToList(Restaurant restaurant)
    {
        _restaurants.Add(restaurant);
    }

    /// <summary>
    /// Adds a new order to a specific restaurant's order list.
    /// </summary>
    /// <param name="restaurant">The restaurant receiving the order.</param>
    /// <param name="order">The <see cref="Order"/> to add.</param>
    public void AddOrderToRestaurantOrderList(Restaurant restaurant, Order order)
    {
        restaurant.AddOrderToList(order);
    }

    /// <summary>
    /// Retrieves all orders currently in the "Cooking" status for a specific restaurant.
    /// </summary>
    /// <param name="restaurant">The restaurant whose cooking orders are being retrieved.</param>
    /// <returns>A list of <see cref="Order"/> objects in the "Cooking" status.</returns>
    public List<Order> GetRestaurantOrdersInCooking(Restaurant restaurant)
    {
        return restaurant.GetAllOrdersThatAreCooking();
    }

    /// <summary>
    /// Retrieves all reviews (ratings) associated with a specific restaurant.
    /// </summary>
    /// <param name="restaurant">The restaurant whose reviews are being retrieved.</param>
    /// <returns>A list of <see cref="Rating"/> objects.</returns>
    public List<Rating> GetRestaurantReviews(Restaurant restaurant)
    {
        return restaurant.Ratings;
    }

    /// <summary>
    /// Updates the status of a specific order for a restaurant.
    /// </summary>
    /// <param name="restaurant">The restaurant processing the order.</param>
    /// <param name="order">The order whose status is to be updated.</param>
    public void UpdateRestaurantOrderStatus(Restaurant restaurant, Order order)
    {
        restaurant.UpdateOrderStatus(order);
    }
}