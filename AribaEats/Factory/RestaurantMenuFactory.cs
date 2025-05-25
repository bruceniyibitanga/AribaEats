using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

/// <summary>
/// Facilitates the creation of menus and workflows for handling restaurant-related operations, such as displaying 
/// restaurant lists, managing orders, and handling deliverer interactions.
/// Implements <see cref="IRestaurantMenuFactory"/>.
/// </summary>
public class RestaurantMenuFactory : IRestaurantMenuFactory
{
    /// <summary>
    /// Manages restaurant-related operations, such as fetching restaurant details and orders.
    /// </summary>
    private readonly RestaurantManager _restaurantManager;

    /// <summary>
    /// Creates screens for order-related workflows (e.g., order placement and processing).
    /// </summary>
    private readonly OrderScreenFactory _orderMenuFactory;

    /// <summary>
    /// Manages order details, including status updates and assignments.
    /// </summary>
    private readonly OrderManager _orderManager;

    /// <summary>
    /// Initialises a new instance of the <see cref="RestaurantMenuFactory"/> class.
    /// Sets up the dependency services required for restaurant and order management.
    /// </summary>
    /// <param name="restaurantManager">The service for managing restaurant data.</param>
    /// <param name="orderMenuFactory">The factory for creating order-related menus.</param>
    /// <param name="orderManager">The service for managing orders.</param>
    public RestaurantMenuFactory(RestaurantManager restaurantManager, OrderScreenFactory orderMenuFactory,
        OrderManager orderManager)
    {
        _restaurantManager = restaurantManager;
        _orderMenuFactory = orderMenuFactory;
        _orderManager = orderManager;
    }

    /// <summary>
    /// Creates a menu for sorting the list of restaurants based on different sort orders.
    /// </summary>
    /// <param name="navigator">The navigation handler for switching between menu screens.</param>
    /// <param name="customer">The customer for whom the menu is being displayed.</param>
    /// <returns>A menu allowing users to select sorting preferences for restaurant lists.</returns>
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

    /// <summary>
    /// Creates a menu displaying a list of restaurants, sorted according to the specified order.
    /// </summary>
    /// <param name="navigator">The navigation handler for switching between menus.</param>
    /// <param name="customer">The customer viewing the restaurant list.</param>
    /// <param name="sortOrder">The sorting preference (e.g., alphabetically, distance, style, rating).</param>
    /// <returns>A menu displaying a sorted list of restaurants.</returns>
    public IMenu CreateRestaurantListMenu(MenuNavigator navigator, Customer customer, string sortOrder)
    {
        // Get the list of all available restaurants
        var restaurants = _restaurantManager.GetRestrauntList();

        // Sort restaurants based on selected sort order and customer location
        var sortedRestaurants = SortRestaurants(
            restaurants, 
            sortOrder, 
            (customer.Location.X, customer.Location.Y).ToTuple()
        );

        // Prepare a list to hold menu items
        var menuItems = new List<IMenuItem>();

        // Configure the table with headers and column widths
        var tableConfig = new TableConfiguration(
            headers: new[] { "Restaurant Name", "Loc", "Dist", "Style", "Rating" },
            columnWidths: new[] { 25, 7, 6, 12, 3 }
        );

        // Add each restaurant as a row in the table
        foreach (var restaurant in sortedRestaurants)
        {
            // Define action when a restaurant is selected
            var actionItem = new ActionMenuItem(restaurant.Name, () =>
            {
                // Navigate to order processing screen for the selected restaurant
                // This screen will be set an anchor to return to.
                navigator.NavigateTo(
                    _orderMenuFactory.CreateOrderProcessingScreen(navigator, customer, restaurant),
                    setAsAnchor: true
                );
            });

            // Prepare the data for this row in the table
            var tableData = new string[]
            {
                restaurant.Name,
                restaurant.Location.ToString(),
                ReusableFunctions.CalculateDistance(
                    (restaurant.Location.X, restaurant.Location.Y).ToTuple(),
                    (customer.Location.X, customer.Location.Y).ToTuple()
                ).ToString(),
                restaurant.Style ?? "Unknown",
                FormatRating(restaurant.Rating)
            };

            // Create a table menu item and add it to the list
            var tableMenuItem = new TableMenuItem(actionItem, tableData);
            menuItems.Add(tableMenuItem);
        }

        // Add a menu item to return to the previous screen
        menuItems.Add(new ActionMenuItem("Return to the previous menu", () =>
        {
            navigator.NavigateHome("customer");
        }));

        // Return the complete table menu
        return new TableMenu(
            "You can order from the following restaurants:",
            menuItems.ToArray(),
            tableConfig
        );
    }


    /// <summary>
    /// Displays the current order screen for a client's restaurant, showing ready orders and their details.
    /// </summary>
    /// <param name="navigator">The navigation handler for switching between menus.</param>
    /// <param name="client">The client managing their restaurant's orders.</param>
    public void ShowCurrentOrderScreen(MenuNavigator navigator, Client client)
    {
        // Retrieve all orders for the client's restaurant that are ready to be cooked
        var readyOrder = _orderManager.GetOrdersReadyForCooking(client.Restaurant.Id);

        // Loop through each ready order
        foreach (var order in readyOrder)
        {
            // Display the order ID, customer name, and order status
            Console.WriteLine($"Order #{order.Id} for {order.Customer.Name}: {order.Status.ToString()}");

            // Group order items by menu item name and sum their quantities
            var groupedItems = order.OrderItems
                .GroupBy(i => i.RestaurantMenuItem.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Quantity = g.Sum(item => item.Quantity)
                });

            // Display each grouped item with its quantity
            foreach (var item in groupedItems)
            {
                Console.WriteLine($"{item.Quantity} x {item.Name}\n");
            }
        }
    }


    /// <summary>
    /// Creates a menu for selecting an order to start cooking.
    /// </summary>
    /// <param name="navigator">The navigation handler for switching between menus.</param>
    /// <param name="client">The client managing their restaurant's orders.</param>
    /// <returns>A menu displaying ready-to-cook orders.</returns>
    public IMenu CreateSelectOrderToCookScreen(MenuNavigator navigator, Client client)
    {
        // Get all orders ready to be cooked for the client's restaurant, sorted by order ID
        var readyOrder = _orderManager
            .GetOrdersReadyForCooking(client.Restaurant.Id)
            .OrderBy(x => int.Parse(x.Id)); // Ensure numeric sorting of string IDs

        var menuItems = new List<IMenuItem>();

        // Create a menu item for each ready order
        foreach (var order in readyOrder)
        {
            menuItems.Add(new ActionMenuItem($"Order #{order.Id} for {order.Customer.Name}", () =>
            {
                // Show order details on screen
                PrepareOrderScreen(order);

                // Update the order status (e.g., mark it as "cooking")
                _orderManager.UpdateStatusOfOrder(order, client.Restaurant);

                // Navigate back to the previous screen
                navigator.NavigateBack();
            }));
        }

        // Add a "Return" option to go back to the previous menu
        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));

        // Create and return the menu with all the order items
        return new ConsoleMenu("Select an order once you are ready to start cooking:", menuItems.ToArray());
    }


    /// <summary>
    /// Creates a menu for the client to mark orders as finished cooking.
    /// </summary>
    /// <param name="navigator">The menu navigator used for screen transitions.</param>
    /// <param name="client">The client user (i.e., restaurant owner/manager).</param>
    /// <returns>
    /// A console menu listing orders currently being cooked,
    /// allowing the client to mark them as ready for collection.
    /// </returns>
    public IMenu CreateOrderFinishedCookingScreen(MenuNavigator navigator, Client client)
    {
        // Get all orders that are currently being cooked, sorted by order ID
        var cookingOrders = _restaurantManager
            .GetRestaurantOrdersInCooking(client.Restaurant)
            .OrderBy(x => int.Parse(x.Id));

        var menuItems = new List<IMenuItem>();

        // Create a menu item for each cooking order
        foreach (var order in cookingOrders)
        {
            menuItems.Add(new ActionMenuItem($"Order #{order.Id} for {order.Customer.Name}", () =>
            {
                // Update the order's status to indicate it's ready
                _orderManager.UpdateStatusOfOrder(order, client.Restaurant);

                Console.WriteLine($"Order #{order.Id} is now ready for collection.");

                // Check if a deliverer has been assigned
                if (order.Deliverer == null)
                    Console.WriteLine("No deliverer has been assigned yet.");

                // Show deliverer arrival tracking screen
                CreateDelivererArrivedScreen(navigator, client, order);
            }));
        }

        // Add option to return to the previous menu
        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));

        // Return the complete menu
        return new ConsoleMenu("Select an order once you have finished preparing it:", menuItems.ToArray());
    }


    /// <summary>
    /// Displays a message to the client indicating the status of the deliverer assigned to a cooked order.
    /// </summary>
    /// <param name="navigator">The menu navigator used for transitioning screens.</param>
    /// <param name="client">The client (i.e., restaurant owner/manager).</param>
    /// <param name="cookedOrder">The order that has just been cooked.</param>
    /// <returns>
    /// A console menu that does not render items but informs the client about deliverer status,
    /// and navigates back to the home screen afterward.
    /// </returns>
    public IMenu CreateDelivererArrivedScreen(MenuNavigator navigator, Client client, Order cookedOrder)
    {
        // Get all assigned orders for the client's restaurant
        var assignedOrders = _orderManager.GetAllAssignedOrders(client.Restaurant); // Dictionary<Deliverer, Order>
        var menuItems = new List<IMenuItem>();

        // Loop through each assigned order
        foreach (var entry in assignedOrders)
        {
            var deliverer = entry.Key;
            var order = entry.Value;
            
            if (cookedOrder.Id == order.Id)
            {
                if (deliverer.Status == DelivererStatus.ArrivedAtRestaurant)
                {
                    // Notify the client that the deliverer has arrived
                    Console.WriteLine(
                        $"Please take it to the deliverer with licence plate {deliverer.LicencePlate}, who is waiting to collect it.");
                }
                else
                {
                    // Notify the client that the deliverer is still on the way
                    Console.WriteLine(
                        $"The deliverer with licence plate {deliverer.LicencePlate} will be arriving soon to collect it.");
                }
            }
        }

        // Automatically return the user to the client home screen
        navigator.NavigateHome("client");

        // Return an empty menu (since this screen only displays a message)
        return new ConsoleMenu("", menuItems.ToArray(), showRowNumbers: false, showLastPrompt: false);
    }

    /// <summary>
    /// Creates a menu screen for the client to handle multiple deliverers who have arrived at the restaurant
    /// and are waiting to collect orders.
    /// </summary>
    /// <param name="navigator">Navigator used to control screen transitions.</param>
    /// <param name="client">The client (restaurant owner/manager).</param>
    /// <returns>
    /// A console menu listing orders for which deliverers have arrived and are ready for pickup.
    /// Selecting an order marks it as picked up if it has been cooked.
    /// </returns>
    public IMenu CreateHandleMultpleDeliverersArrivalScreen(MenuNavigator navigator, Client client)
    {
        // Retrieve all orders assigned to deliverers for the client's restaurant
        var assignedOrders = _orderManager.GetAllAssignedOrders(client.Restaurant); // Dictionary<Deliverer, Order>

        // Filter out only those where the deliverer has arrived at the restaurant
        var arrivedOrders = assignedOrders
            .Where(entry => entry.Key.Status == DelivererStatus.ArrivedAtRestaurant)
            .ToList();

        Console.WriteLine("These deliverers have arrived and are waiting to collect orders.");

        var menuItems = new List<IMenuItem>();
        
        // Show orders that
        for (int i = 0; i < arrivedOrders.Count; i++)
        {
            var deliverer = arrivedOrders[i].Key;
            var order = arrivedOrders[i].Value;

            // Skip orders already picked up
            if (order.Status == OrderStatus.PickedUp) continue;

            // Add a menu item for each order that can be collected
            menuItems.Add(new ActionMenuItem(
                $"Order #{order.Id} for {order.Customer.Name} (Deliverer licence plate: {deliverer.LicencePlate}) (Order status: {order.Status})",
                () =>
                {
                    // If the order hasn't been cooked yet, inform the user and return home
                    if (order.Status != OrderStatus.Cooked)
                    {
                        Console.WriteLine("This order has not yet been cooked.");
                        navigator.NavigateHome("client");
                        return;
                    }

                    // Mark the order as picked up and notify the client
                    Console.WriteLine($"Order #{order.Id} is now marked as being delivered.");
                    _orderManager.UpdateStatusOfOrder(order, client.Restaurant);

                    // Return to client home screen
                    navigator.NavigateHome("client");
                }));
        }

        // Add an option to go back to the previous menu
        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));

        return new ConsoleMenu("Select an order to indicate that the deliverer has collected it:", menuItems.ToArray());
    }

    /// <summary>
    /// Displays the order details and prompts the user to prepare the order.
    /// </summary>
    /// <param name="order">The order to prepare.</param>
    private void PrepareOrderScreen(Order order)
    {
        // TODO: This method currently only prints the info and does not store the status update.
        Console.WriteLine(
            $"Order #{order.Id} is now marked as cooking. Please prepare the order, then mark it as finished cooking:");

        // Group order items by their menu item name and sum quantities for display
        var groupedItems = order.OrderItems
            .GroupBy(i => i.RestaurantMenuItem.Name)
            .Select(g => new
            {
                Name = g.Key,
                Quantity = g.Sum(item => item.Quantity)
            });

        // Display the grouped items with their quantities
        foreach (var item in groupedItems)
        {
            Console.WriteLine($"{item.Quantity} x {item.Name}");
        }
    }

    /// <summary>
    /// Sorts a collection of restaurants based on the specified sort order.
    /// </summary>
    /// <param name="restaurants">The list of restaurants to sort.</param>
    /// <param name="sortOrder">The sort criteria: "alphabetically", "distance", "style", or "rating".</param>
    /// <param name="customerLocation">The customer's location as a tuple (X, Y), used for distance sorting.</param>
    /// <returns>A sorted enumerable of restaurants.</returns>
    private IEnumerable<Restaurant> SortRestaurants(IEnumerable<Restaurant> restaurants, string sortOrder, Tuple<int, int> customerLocation)
    {
        return sortOrder.ToLower() switch
        {
            // Sort by restaurant name alphabetically (A-Z)
            "alphabetically" => restaurants.OrderBy(r => r.Name, StringComparer.Ordinal),

            // Sort by distance from customer location, then by name
            "distance" => restaurants
                .OrderBy(r => ReusableFunctions.CalculateDistance(
                    customerLocation, 
                    new Tuple<int, int>(r.Location?.X ?? 0, r.Location?.Y ?? 0)))
                .ThenBy(r => r.Name, StringComparer.Ordinal),

            // Sort by restaurant style using a custom style order, then by name
            "style" => restaurants
                .OrderBy(r => GetStyleOrder(r.Style))
                .ThenBy(r => r.Name, StringComparer.Ordinal),

            // Sort by rating: rated restaurants first, then descending rating, then name
            "rating" => restaurants
                .OrderByDescending(r => r.Rating.HasValue ? 1 : 0)  // Rated restaurants come first
                .ThenByDescending(r => r.Rating ?? 0)               // Higher rating first
                .ThenBy(r => r.Name, StringComparer.Ordinal),       // Then by name

            // Default fallback: sort alphabetically by name
            _ => restaurants.OrderBy(r => r.Name, StringComparer.Ordinal)
        };
    }

    /// <summary>
    /// Returns an integer representing the sort order priority of a restaurant style.
    /// </summary>
    /// <param name="style">The style of the restaurant as a string.</param>
    /// <returns>An integer representing the priority of the style; unknown styles get the highest value (7).</returns>
    private int GetStyleOrder(string style)
    {
        // Map known styles to specific order values; unknown styles get 7 (lowest priority)
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
    
    /// <summary>
    /// Formats a nullable rating value for display.
    /// </summary>
    /// <param name="rating">The rating to format, which can be null.</param>
    /// <returns>A string representation of the rating formatted to one decimal place, or "-" if no rating.</returns>
    private string FormatRating(double? rating)
    {
        // Return "-" if rating is null or zero, otherwise format with one decimal place
        return (rating == null || rating == 0.0) ? "-" : rating.Value.ToString("F1");
    }

}