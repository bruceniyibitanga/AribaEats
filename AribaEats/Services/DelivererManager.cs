using AribaEats.Models;

namespace AribaEats.Services;

public class DelivererManager
{
    private readonly OrderManager _orderManager;
    private readonly RestaurantManager _restaurantManager;
    
    private Dictionary<Deliverer, Order> _assignedOrders = new Dictionary<Deliverer, Order>();
    
    public DelivererManager(OrderManager orderManager, RestaurantManager restaurantManager)
    {
        _orderManager = orderManager;
        _restaurantManager = restaurantManager;
    }

    public bool AcceptDelivery(Deliverer deliverer, Order order)
    {
        // Add deliverer to assignedOrders
        _assignedOrders.Add(deliverer, order);
        
        int assingmentId = _assignedOrders.Count;

        Console.WriteLine(
            $"Thanks for accepting the order. Please head to {order.Restaurant.Name} at {order.Restaurant.Location.X},{order.Restaurant.Location.Y} to pick it up.");
        return true;
    }

    public bool IsAvailable(Deliverer deliverer)
    {
        return !_assignedOrders.ContainsKey(deliverer);
    }
}