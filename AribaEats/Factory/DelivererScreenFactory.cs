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
}