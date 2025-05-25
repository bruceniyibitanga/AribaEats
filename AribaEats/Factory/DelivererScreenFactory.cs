using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

/// <summary>
/// Factory class responsible for creating and managing deliverer-specific screens and user interfaces.
/// Handles the delivery workflow including order acceptance, restaurant arrival, and delivery completion.
/// </summary>
public class DelivererScreenFactory
{
    #region Private Fields

    private readonly OrderManager _orderManager;
    private readonly DelivererManager _delivererManager;

    #endregion

    #region Constructor

    /// <summary>
    /// Initialises a new instance of the DelivererScreenFactory with required dependencies.
    /// </summary>
    /// <param name="orderManager">Service for managing orders and their statuses</param>
    /// <param name="delivererManager">Service for managing deliverer operations and assignments</param>
    public DelivererScreenFactory(OrderManager orderManager, DelivererManager delivererManager)
    {
        _orderManager = orderManager;
        _delivererManager = delivererManager;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a screen showing available deliveries that the deliverer can accept.
    /// Includes location input, distance calculations, and order selection in a table format.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="deliverer">The deliverer viewing available orders</param>
    /// <returns>A table menu displaying available deliveries with distance information</returns>
    public IMenu CreateAvailableDeliveriesScreen(MenuNavigator navigator, Deliverer deliverer)
    {
        // Clean up deliverer status if they completed their last delivery but status wasn't updated
        if (deliverer.Status == DelivererStatus.Delivered && 
            _delivererManager.GetAllOrdersAssignedToDriver(deliverer).Count == 0)
        {
            _delivererManager.UpdateDelivererStatus(deliverer);
        }

        // Get deliverer's current location for distance calculations
        // TODO: THIS NEEDS TO BE REFACTORED OUT SOMEHOW - location input should be handled elsewhere
        bool isValid = false;
        Tuple<int, int> loc;
        while (!isValid)
        {
            Console.WriteLine("Please enter your location (in the form of X,Y):");
            string input = Console.ReadLine();
            string[] location = input.Split(',');
            isValid = IsValidLocation(location);
            
            if (isValid)
            {
                // Parse and update deliverer's location
                loc = (Convert.ToInt32(location[0]), Convert.ToInt32(location[1])).ToTuple();
                deliverer.Location.X = loc.Item1;
                deliverer.Location.Y = loc.Item2;
            }
            else
            {
                Console.WriteLine("Invalid location.");
            }
        }
        
        // Get all orders that are ready for delivery
        var orders = _orderManager.GetAllDeliverableOrders();
        List<IMenuItem> menuItems = new List<IMenuItem>();
        
        // Configure table display for order information
        var tableConfig = new TableConfiguration(
            headers: new[] { "Order", "Restaurant Name", "Loc", "Customer Name", "Loc", "Dist" },
            columnWidths: new[] { 10, 22, 7, 17, 7, 0 }
        );

        // Create menu items for each available order
        foreach (var order in orders)
        {
            // Create action for accepting this delivery
            var actionItem = new ActionMenuItem(order.CustomerId, () =>
            {
                // Assign the order to this deliverer
                _delivererManager.AcceptDelivery(deliverer, order);
                _delivererManager.UpdateDelivererStatus(deliverer);
                
                // Provide pickup instructions
                Console.WriteLine($"Thanks for accepting the order. Please head to {order.Restaurant.Name} at {order.Restaurant.Location.X},{order.Restaurant.Location.Y} to pick it up.");
                navigator.NavigateHome("deliverer");
            });
            
            // Calculate total delivery distance (deliverer -> restaurant -> customer)
            int toRestaurant = Helper.ReusableFunctions.CalculateDistance(
                (deliverer.Location.X, deliverer.Location.Y).ToTuple(), 
                (order.Restaurant.Location.X, order.Restaurant.Location.Y).ToTuple());
            
            int toCustomer = Helper.ReusableFunctions.CalculateDistance(
                (order.Restaurant.Location.X, order.Restaurant.Location.Y).ToTuple(), 
                (order.Customer.Location.X, order.Customer.Location.Y).ToTuple());
            
            int totalDistance = toRestaurant + toCustomer;

            // Prepare data for table display
            var tableData = new string[]
            {
                order.Id,
                order.Restaurant.Name,
                order.Restaurant.Location.ToString(),
                order.Customer.Name,
                order.Customer.Location.ToString(),
                totalDistance.ToString(),
            };

            // Create table menu item combining action and display data
            var tableMenuItem = new TableMenuItem(actionItem, tableData);
            menuItems.Add(tableMenuItem);
        }
        
        // Add navigation option
        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));
        
        // Create and return the table menu
        var menu = new TableMenu(
            "The following orders are available for delivery. Select an order to accept it:", 
            menuItems.ToArray(), 
            tableConfig);
        
        return menu;
    }

    /// <summary>
    /// Handles the screen shown when a deliverer arrives at a restaurant to pick up an order.
    /// Validates the deliverer's status and provides pickup instructions.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="deliverer">The deliverer who has arrived at the restaurant</param>
    /// <returns>A simple menu that immediately navigates back after displaying information</returns>
    public IMenu ShowArrivedAtRestaurantScreen(MenuNavigator navigator, Deliverer deliverer)
    {
        // Get the deliverer's assigned orders
        List<Order> orders = _delivererManager.GetAllOrdersAssignedToDriver(deliverer);

        // Validation: Check if deliverer has accepted an order
        if (orders == null || orders.Count == 0)
        {
            Console.WriteLine("You have not yet accepted an order.");
            navigator.NavigateHome("deliverer");
        }

        // Assuming one active order per deliverer at a time
        Order order = orders[0];

        // Validation: Check if order has already been picked up
        if (order.Status == OrderStatus.InTransit | 
            order.Status == OrderStatus.PickedUp | 
            order.Status == OrderStatus.Delivered)
        {
            Console.WriteLine("You have already picked up this order.");
            navigator.NavigateHome("deliverer");
        }

        // Validation: Check if deliverer already marked as arrived
        if (deliverer.Status == DelivererStatus.ArrivedAtRestaurant)
        {
            Console.WriteLine("You already indicated that you have arrived at this restaurant.");
            navigator.NavigateHome("deliverer");
        }

        // Valid arrival - provide confirmation and instructions
        Console.WriteLine($"Thanks. We have informed {order.Restaurant.Name} that you have arrived and are ready to pick up order #{order.Id}.");
        Console.WriteLine("Please show the staff this screen as confirmation.");

        // Check if order is still being prepared
        if (order.Status == OrderStatus.Ordered || order.Status == OrderStatus.Cooking)
        {
            Console.WriteLine("The order is still being prepared, so please wait patiently until it is ready.");
        }

        // Provide delivery destination information
        var customer = order.Customer;
        Console.WriteLine($"When you have the order, please deliver it to {customer.Name} at {customer.Location.X},{customer.Location.Y}.");

        // Update deliverer status to indicate arrival at restaurant
        deliverer.UpdateDelivererStatus(deliverer);
        
        // Navigate back to deliverer home menu
        navigator.NavigateHome("deliverer");

        // Return a minimal menu that doesn't display options (used for immediate navigation scenarios)
        return new ConsoleMenu("", new IMenuItem[] { new ActionMenuItem("", () => { }) }, 
            showRowNumbers: false, showLastPrompt: false);
    }

    /// <summary>
    /// Creates a screen for completing a delivery after the deliverer has delivered the order to the customer.
    /// Handles final status updates and cleanup of order assignments.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="deliverer">The deliverer completing the delivery</param>
    /// <returns>A simple menu that immediately navigates back after processing completion</returns>
    public IMenu CreateCompleteDeliveryScreen(MenuNavigator navigator, Deliverer deliverer)
    {
        // Get the deliverer's assigned orders
        List<Order> orders = _delivererManager.GetAllOrdersAssignedToDriver(deliverer);

        // Validation: Check if deliverer has accepted an order
        if (orders == null || orders.Count == 0)
        {
            Console.WriteLine("You have not yet accepted an order.");
            navigator.NavigateHome("deliverer");
        }

        // Assuming one active order per deliverer at a time
        Order order = orders[0];

        // Validation: Check if order has been picked up (required before delivery completion)
        if (order.Status != OrderStatus.PickedUp)
        {
            Console.WriteLine("You have not yet picked up this order.");
            navigator.NavigateHome("deliverer");
        }

        // Valid delivery completion
        Console.WriteLine("Thank you for making the delivery.");

        // Update order status to delivered
        _orderManager.UpdateStatusOfOrder(order, order.Restaurant);

        // Update deliverer status (likely to Available/Free)
        _delivererManager.UpdateDelivererStatus(deliverer);
        
        // Clean up: Remove the completed order from deliverer's assigned orders
        _delivererManager.RemoveOrderAssignment(deliverer);
        
        // Navigate back to deliverer home menu
        navigator.NavigateHome("deliverer");
        
        // Return a minimal menu that doesn't display options (used for immediate navigation scenarios)
        return new ConsoleMenu("", new IMenuItem[] { new ActionMenuItem("", () => { }) }, 
            showRowNumbers: false, showLastPrompt: false);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Validates that a location input string array contains exactly two valid integers.
    /// </summary>
    /// <param name="location">Array of strings representing X,Y coordinates</param>
    /// <returns>True if the location format is valid (two integers), false otherwise</returns>
    private bool IsValidLocation(string[] location)
    {
        // Must have exactly two components (X and Y coordinates)
        if (location.Length != 2) return false;

        // Each component must be a valid integer
        foreach (string value in location)
        {
            bool canParse = int.TryParse(value, out int result);
            if (!canParse) return false;
        }

        return true;
    }

    #endregion
}