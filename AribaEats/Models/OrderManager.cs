namespace AribaEats.Models;

public class OrderManager
{
    private List<Order> _orders = new();
    private RestaurantManager _restaurantManager;

    public OrderManager(RestaurantManager restaurantManager)
    {
        _restaurantManager = restaurantManager;
    }

    public void AddOrder(Order order) => _orders.Add(order);

    public List<string> GetOrderStatuses(string customerId)
    {
        var customerOrders = _orders.Where(o => o.CustomerId == customerId);

        var statusMessages = customerOrders.Select(order =>
        {
            var restaurant = _restaurantManager.GetRestaurantById(order.RestaurantId);
            var restaurantName = restaurant?.Name ?? "Unknown Restaurant";

            return $"Order #{order.Id} from {restaurantName}: {order.Status}";
        });
        
        return statusMessages.ToList();
    }
}

