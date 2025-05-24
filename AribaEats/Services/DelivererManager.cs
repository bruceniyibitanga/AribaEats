using AribaEats.Models;

namespace AribaEats.Services;

public class DelivererManager
{
    private readonly RestaurantManager _restaurantManager;
    
    private Dictionary<Deliverer, Order> _assignedOrders = new Dictionary<Deliverer, Order>();
    
    public DelivererManager(RestaurantManager restaurantManager)
    {
        _restaurantManager = restaurantManager;
    }

    public bool AcceptDelivery(Deliverer deliverer, Order order)
    {
        // Add deliverer to assignedOrders Dictonary
        order.Deliverer = deliverer;
        order.DelivererId = deliverer.Id;
        _assignedOrders.Add(deliverer, order);
        // Placeholder for potential delivery ID
        int assingmentId = _assignedOrders.Count;
        
        return true;
    }

    public void UpdateDelivererStatus(Deliverer deliverer)
    {
        deliverer.UpdateDelivererStatus(deliverer);
    }
    public string GetCurrentDeliveryStatus(Deliverer deliverer)
    {
        if (!_assignedOrders.TryGetValue(deliverer, out var order)) return "";

        var restaurant = order.Restaurant;
        var customer = order.Customer;

        return $"Current delivery:\n" +
               $"Order #{order.Id} from {restaurant.Name} at {restaurant.Location.X},{restaurant.Location.Y}.\n" +
               $"To be delivered to {customer.Name} at {customer.Location.X},{customer.Location.Y}.";
    }

    public Dictionary<Deliverer, Order> GetAllAssignedOrdersToRestaurant(Restaurant restaurant)
    {
        return _assignedOrders
            .Where(entry => entry.Value.Restaurant == restaurant)
            .ToDictionary(entry => entry.Key, entry => entry.Value);
    }
    
    public List<Order> GetAllOrdersAssignedToDriver(Deliverer deliverer)
    {
        if (_assignedOrders.TryGetValue(deliverer, out var order))
        {
            return new List<Order> { order };
        }
        return new List<Order>();
    }
    
    public bool IsAvailable(Deliverer deliverer)
    {
        return !_assignedOrders.ContainsKey(deliverer);
    }

    public void RemoveOrderAssignment(Deliverer deliverer)
    {
        _assignedOrders.Remove(deliverer);
        
        // Add Order to customers Order List
        
        
        // After order has been delivered and remove the assignment the deliverer should be marked as available to deliver again.
        deliverer.UpdateDelivererStatus(deliverer);
    }
}