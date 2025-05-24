using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

public class DelivererScreenFactory
{
    private readonly OrderManager _orderManager;
    private readonly DelivererManager _delivererManager;
    public DelivererScreenFactory(OrderManager orderManager, DelivererManager delivererManager)
    {
        _orderManager = orderManager;
        _delivererManager = delivererManager;
    }
    public IMenu CreateAvailableDeliveriesScreen(MenuNavigator navigator, Deliverer deliverer)
    {
        
        // TODO: THIS NEEDS TO BE REFACTORED OUT SOMEHOW
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
                loc = (Convert.ToInt32(location[0]), Convert.ToInt32(location[1])).ToTuple();
            }
            else
            {
                Console.WriteLine("Invalid location.");
            }
        }

        var orders = _orderManager.GetAllDeliverableOrders();
        List<IMenuItem> menuItems = new List<IMenuItem>();
        
        var tableConfig = new TableConfiguration(
            headers: new[] { "Order", "Restaurant Name", "Loc", "Customer Name", "Loc", "Dist" },
            columnWidths: new[] { 10, 22, 7, 17, 7, 0 }
        );
        

        foreach (var order in orders)
        {
            var actionItem = new ActionMenuItem(order.CustomerId, () =>
            {
                _delivererManager.AcceptDelivery(deliverer, order);
                _delivererManager.UpdateDelivererStatus(deliverer);
                Console.WriteLine($"Thanks for accepting the order. Please head to {order.Restaurant.Name} at {order.Restaurant.Location.X},{order.Restaurant.Location.Y} to pick it up.");
                navigator.NavigateHome("deliverer");
            });
            
            var distance = Helper.ReusableFunctions.CalculateDistance((order.Restaurant.Location.X, order.Restaurant.Location.Y).ToTuple(), (order.Customer.Location.X, order.Customer.Location.Y).ToTuple());


            var tableData = new string[]
            {
                order.Id,
                order.Restaurant.Name,
                order.Restaurant.Location.ToString(),
                order.Customer.Name,
                order.Customer.Location.ToString(),
                (distance * 2).ToString(),
            };

            var tableMenuItem = new TableMenuItem(actionItem, tableData);
            menuItems.Add(tableMenuItem);
        }
        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));
        var menu = new TableMenu("The following orders are available for delivery. Select an order to accept it:", menuItems.ToArray(), tableConfig);
        
        return menu;
    }

    public IMenu ShowArrivedAtRestaurantScreen(MenuNavigator navigator, Deliverer deliverer)
    {
        List<Order> orders = _delivererManager.GetAllOrdersAssignedToDriver(deliverer);

        // No order accepted
        if (orders == null || orders.Count == 0)
        {
            Console.WriteLine("You have not yet accepted an order.");
            navigator.NavigateHome("deliverer");
        }

        Order order = orders[0]; // Assuming one active order per deliverer

        // Already picked up
        if (order.Status == OrderStatus.InTransit | order.Status == OrderStatus.PickedUp | order.Status == OrderStatus.Delivered)
        {
            Console.WriteLine("You have already picked up this order.");
            navigator.NavigateHome("deliverer");
        }

        // Already marked as arrived
        if (deliverer.Status == DelivererStatus.ArrivedAtRestaurant)
        {
            Console.WriteLine("You already indicated that you have arrived at this restaurant.");
            navigator.NavigateHome("deliverer");
        }

        // Valid arrival
        Console.WriteLine($"Thanks. We have informed {order.Restaurant.Name} that you have arrived and are ready to pick up order #{order.Id}.");
        Console.WriteLine("Please show the staff this screen as confirmation.");

        if (order.Status == OrderStatus.Ordered || order.Status == OrderStatus.Cooking)
        {
            Console.WriteLine("The order is still being prepared, so please wait patiently until it is ready.");
        }

        var customer = order.Customer;
        Console.WriteLine($"When you have the order, please deliver it to {customer.Name} at {customer.Location.X},{customer.Location.Y}.");

        // Mark deliverer as arrived at restaurant
        //TODO: DELIVERER STATUS = ARRIVED ARRIVED AT RESTAURANT
        deliverer.UpdateDelivererStatus(deliverer);
        
        navigator.NavigateHome("deliverer");

        return new ConsoleMenu("", new IMenuItem[]{new ActionMenuItem("", () => { })}, showRowNumbers:false, showLastPrompt:false);
    }

    public IMenu CreateCompleteDeliveryScreen(MenuNavigator navigator, Deliverer deliverer)
    {
        
        List<Order> orders = _delivererManager.GetAllOrdersAssignedToDriver(deliverer);

        // No order accepted
        if (orders == null || orders.Count == 0)
        {
            Console.WriteLine("You have not yet accepted an order.");
            navigator.NavigateHome("deliverer");
        }

        Order order = orders[0]; // Assuming one active order per deliverer

        // Order not picked up yet
        if (order.Status != OrderStatus.PickedUp)
        {
            Console.WriteLine("You have not yet picked up this order.");
            navigator.NavigateHome("deliverer");
        }

        // Valid delivery completion
        Console.WriteLine("Thank you for making the delivery.");

        // Update order and deliverer status
        _orderManager.UpdateStatusOfOrder(order, order.Restaurant);

        _delivererManager.UpdateDelivererStatus(deliverer);
        
        // Remove the order from _assignedOrders
        _delivererManager.RemoveOrderAssignment(deliverer);
        
        navigator.NavigateHome("deliverer");
        return new ConsoleMenu("", new IMenuItem[]{new ActionMenuItem("", () => { })}, showRowNumbers:false, showLastPrompt:false);
    }
    
    // TODO: This needs to be refactored out. Repetition of Logic in User Validation Service
    private bool IsValidLocation(string[] location)
    {
        if (location.Length != 2) return false;

        foreach (string value in location)
        {
            bool canParse = int.TryParse(value, out int result);

            if (!canParse) return false;
        }

        return true;
    }

    private bool DriverConfirmationationScreen()
    {
        return true;
    }
}