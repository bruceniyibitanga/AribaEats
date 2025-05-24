using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

public class RestaurantMenuFactory : IRestaurantMenuFactory
{
    private readonly RestaurantManager _restaurantManager;
    private readonly OrderScreenFactory _orderMenuFactory;
    private readonly OrderManager _orderManager;

    public RestaurantMenuFactory(RestaurantManager restaurantManager, OrderScreenFactory orderMenuFactory,
        OrderManager orderManager)
    {
        _restaurantManager = restaurantManager;
        _orderMenuFactory = orderMenuFactory;
        _orderManager = orderManager;
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
        var sortedRestaurants = SortRestaurants(restaurants, sortOrder, (customer.Location.X, customer.Location.Y).ToTuple());

        var menuItems = new List<IMenuItem>();
        var tableConfig = new TableConfiguration(
            headers: new[] { "Restaurant Name", "Loc", "Dist", "Style", "Rating" },
            columnWidths: new[] { 25, 7, 6, 12, 3 }
        );

        // Add restaurant items as table items
        foreach (var restaurant in sortedRestaurants)
        {
            var actionItem = new ActionMenuItem(restaurant.Name, () =>
            {
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

        menuItems.Add(new ActionMenuItem("Return to the previous menu",() => {navigator.NavigateHome("customer");}));
        return new TableMenu("You can order from the following restaurants:", menuItems.ToArray(), tableConfig);
    }

    public void ShowCurrentOrderScreen(MenuNavigator navigator, Client client)
    {
        var readyOrder = _orderManager.GetOrdersReadyForCooking(client.Restaurant.Id);
        var menuItems = new List<IMenuItem>();

        foreach (var order in readyOrder)
        {
            Console.WriteLine($"Order #{order.Id} for {order.Customer.Name}: {order.Status.ToString()}");

            var groupedItems = order.OrderItems
                .GroupBy(i => i.RestaurantMenuItem.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Quantity = g.Sum(item => item.Quantity)
                });

            foreach (var item in groupedItems)
            {
                Console.WriteLine($"{item.Quantity} x {item.Name}\n");
            }

            // menuItems.Add(new ActionMenuItem($"Order #{order.Id} for {order.Customer.Name}",() => {}));
        }
    }

    public IMenu CreateSelectOrderToCookScreen(MenuNavigator navigator, Client client)
    {
        var readyOrder = _orderManager.GetOrdersReadyForCooking(client.Restaurant.Id);
        var menuItems = new List<IMenuItem>();

        foreach (var order in readyOrder)
        {
            menuItems.Add(new ActionMenuItem($"Order #{order.Id} for {order.Customer.Name}", () =>
            {
                PrepareOrderScreen(order);
                _orderManager.UpdateStatusOfOrder(order, client.Restaurant);
                navigator.NavigateBack();
            }));
        }

        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));
        return new ConsoleMenu("Select an order once you are ready to start cooking:", menuItems.ToArray());
    }

    public IMenu CreateOrderFinishedCookingScreen(MenuNavigator navigator, Client client)
    {
        var cookingOrders = _restaurantManager.GetRestaurantOrdersInCooking(client.Restaurant);
        var menuItems = new List<IMenuItem>();
        foreach (var order in cookingOrders)
        {
            menuItems.Add(new ActionMenuItem($"Order #{order.Id} for {order.Customer.Name}", () =>
            {
                _orderManager.UpdateStatusOfOrder(order, client.Restaurant);

                Console.WriteLine($"Order #{order.Id} is now ready for collection.");

                CreateDelivererArrivedScreen(navigator, client);
            }));
        }

        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));
        return new ConsoleMenu("Select an order once you have finished preparing it:", menuItems.ToArray());
    }

    public IMenu CreateDelivererArrivedScreen(MenuNavigator navigator, Client client)
    {
        var assignedOrders = _orderManager.GetAllAssignedOrders(client.Restaurant); // Dictionary<Deliverer, Order>
        var menuItems = new List<IMenuItem>();

        if (assignedOrders.Count == 0)
        {
            Console.WriteLine("No deliverer has been assigned yet.");
            navigator.NavigateHome("client");
        }
        else
        {
            foreach (var entry in assignedOrders)
            {
                var deliverer = entry.Key;
                var order = entry.Value;

                if (deliverer.Status == DelivererStatus.ArrivedAtRestaurant)
                {
                    Console.WriteLine(
                        $"Please take it to the deliverer with licence plate {deliverer.LicencePlate}, who is waiting to collect it.");
                }
                else
                {
                    Console.WriteLine(
                        $"The deliverer with licence plate {deliverer.LicencePlate} will be arriving soon to collect it.");
                }
            }

            navigator.NavigateHome("client");
        }
        return new ConsoleMenu("", menuItems.ToArray(), showRowNumbers:false, showLastPrompt:false);
    }

    public IMenu CreateHandleMultpleDeliverersArrivalScreen(MenuNavigator navigator, Client client)
    {
        var assignedOrders = _orderManager.GetAllAssignedOrders(client.Restaurant); // Dictionary<Deliverer, Order>
        var arrivedOrders = assignedOrders
            .Where(entry => entry.Key.Status == DelivererStatus.ArrivedAtRestaurant)
            .ToList();

        if (arrivedOrders.Count == 0)
        {
            Console.WriteLine("No deliverer has arrived yet.");
            navigator.NavigateHome("client");
        }

        Console.WriteLine("These deliverers have arrived and are waiting to collect orders.");
        
        var menuItems = new List<IMenuItem>();

        for (int i = 0; i < arrivedOrders.Count; i++)
        {
            var deliverer = arrivedOrders[i].Key;
            var order = arrivedOrders[i].Value;

            menuItems.Add(new ActionMenuItem(
            $"Order #{order.Id} for {order.Customer.Name} (Deliverer license plate: {deliverer.LicencePlate}) (Order status: {order.Status})"
            , () =>
            {
                // If a user selects order... Display the staus of order
                if (order.Status != OrderStatus.Cooked)
                {
                    Console.WriteLine("This order has not yet been cooked");
                    navigator.NavigateHome("client");
                }

                Console.WriteLine($"Order #{order.Id} is now marked as being delivered.");
                _orderManager.UpdateStatusOfOrder(order, client.Restaurant);
                navigator.NavigateHome("client");
            }));
        }
        
        //TODO: DELIVERER STATUS = EN ROUTE
        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));
        return new ConsoleMenu("Select an order to indicate that the deliverer has collected it:", menuItems.ToArray());
    }

    private void PrepareOrderScreen(Order order)
    {
        // TODO: This needs to be fixed so to store the value after processing.
        Console.WriteLine(
            $"Order #{order.Id} is now marked as cooking. Please prepare the order, then mark it as finished cooking:");
        {
            var groupedItems = order.OrderItems
                .GroupBy(i => i.RestaurantMenuItem.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Quantity = g.Sum(item => item.Quantity)
                });

            foreach (var item in groupedItems)
            {
                Console.WriteLine($"{item.Quantity} x {item.Name}");
            }
        }
    }

    private IEnumerable<Restaurant> SortRestaurants(IEnumerable<Restaurant> restaurants, string sortOrder, Tuple<int, int> customerLocation)
    {
        return sortOrder.ToLower() switch
        {
            "alphabetically" => restaurants.OrderBy(r => r.Name, StringComparer.Ordinal),
        
            "distance" => restaurants
                .OrderBy(r => ReusableFunctions.CalculateDistance(customerLocation, new Tuple<int, int>(r.Location?.X ?? 0, r.Location?.Y ?? 0)))
                .ThenBy(r => r.Name, StringComparer.Ordinal),
        
            "style" => restaurants
                .OrderBy(r => GetStyleOrder(r.Style))
                .ThenBy(r => r.Name, StringComparer.Ordinal),
        
            "rating" => restaurants
                .OrderByDescending(r => r.Rating.HasValue ? 1 : 0) // Rated restaurants first
                .ThenByDescending(r => r.Rating ?? 0) // Then by rating value
                .ThenBy(r => r.Name, StringComparer.Ordinal), // Then by name
        
            _ => restaurants.OrderBy(r => r.Name, StringComparer.Ordinal)
        };
    }

    private int GetStyleOrder(string style)
    {
        return style?.ToLower() switch
        {
            "italian" => 1,
            "french" => 2,
            "chinese" => 3,
            "japanese" => 4,
            "american" => 5,
            "australian" => 6,
            _ => 7 // Unknown styles go last
        };
    }
    
    private string FormatRating(double? rating)
    {
        return (rating == null || rating == 0.0) ? "-" : rating.Value.ToString("F1");
    }
}