using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

public class RestaurantMenuFactory :IRestaurantMenuFactory
{
    private readonly RestaurantManager _restaurantManager;
    private readonly OrderScreenFactory _orderMenuFactory;

    public RestaurantMenuFactory(RestaurantManager restaurantManager, OrderScreenFactory orderMenuFactory)
    {
        _restaurantManager = restaurantManager;
        _orderMenuFactory = orderMenuFactory;
    }

    public IMenu CreateRestaurantSortMenu(MenuNavigator navigator, Customer customer)
    {
        return new ConsoleMenu("How would you like the list of restaurants ordered?", new IMenuItem[]
        {
            new ActionMenuItem("Sorted alphabetically by name", () => 
                navigator.NavigateTo(CreateRestaurantListMenu(navigator, customer, "alphabetically"))),
            new ActionMenuItem("Sorted by distance", () => 
                navigator.NavigateTo(CreateRestaurantListMenu(navigator, customer, "distance"))),
            new ActionMenuItem("Sorted by style", () => 
                navigator.NavigateTo(CreateRestaurantListMenu(navigator, customer, "style"))),
            new ActionMenuItem("Sorted by average rating", () => 
                navigator.NavigateTo(CreateRestaurantListMenu(navigator, customer, "rating"))),
            new BackMenuItem("Return to the previous menu", navigator)
        });
    }

    public IMenu CreateRestaurantListMenu(MenuNavigator navigator, Customer customer, string sortOrder)
    {
        var restaurants = _restaurantManager.GetRestrauntList();
        var sortedRestaurants = SortRestaurants(restaurants, sortOrder);
        
        var menuItems = new List<IMenuItem>();
        var tableConfig = new TableConfiguration(
            headers: new[] { "Restaurant Name", "Loc", "Dist", "Style", "Rating" },
            columnWidths: new[] { 25, 7, 6, 12, 3 }
        );
        
        // Add restaurant items as table items
        foreach (var restaurant in sortedRestaurants)
        {
            var actionItem = new ActionMenuItem(restaurant.Name, () => {
                // Navigate to restaurant menu or order page
                navigator.NavigateTo(_orderMenuFactory.CreateOrderProcessingScreen(navigator, customer, restaurant),
                    setAsAnchor: true);
            });
            
            var tableData = new string[]
            {
                restaurant.Name,
                restaurant.Location.ToString(),
                ReusableFunctions.CalculateDistance((restaurant.Location.X, restaurant.Location.Y).ToTuple(),
                    (customer.Location.X, customer.Location.Y).ToTuple()).ToString(),
                restaurant.Style ?? "Unknown",
                FormatRating(restaurant.Rating)
            };
            
            var tableMenuItem = new TableMenuItem(actionItem, tableData);
            menuItems.Add(tableMenuItem);
        }
        // Console.WriteLine();
        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));
        return new TableMenu("You can order from the following restaurants:", menuItems.ToArray(), tableConfig);
    }

    private IEnumerable<Restaurant> SortRestaurants(IEnumerable<Restaurant> restaurants, string sortOrder)
    {
        return sortOrder.ToLower() switch
        {
            "alphabetically" => restaurants.OrderBy(r => r.Name),
            "distance" => restaurants.OrderBy(r => r.Location?.X + r.Location?.Y), // Simplified distance
            "style" => restaurants.OrderBy(r => r.Style),
            "rating" => restaurants.OrderByDescending(r => r.Rating),
            _ => restaurants.OrderBy(r => r.Name)
        };
    }

    private string FormatRating(double? rating)
    {
        return (rating == null || rating == 0.0) ? "-" : rating.Value.ToString("F1");
    }

}