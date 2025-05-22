using AribaEats.Models;

namespace AribaEats.Services;

public class OrderManager
{
    private readonly UserManager _userManager;
    public OrderManager(RestaurantManager restaurantManager, UserManager userManager)
    {
        _restaurantManager = restaurantManager;
        _userManager = userManager;
    }
    
    private List<Order> _orders = new();
    private RestaurantManager _restaurantManager;
    public List<string> GetOrderStatuses(string customerId)
    {
        var customerOrders = _orders.Where(o => o.CustomerId == customerId).ToList();

        var statusMessages = customerOrders.Select(order =>
        {
            var restaurant = _restaurantManager.GetRestaurantById(order.RestaurantId);
            var restaurantName = restaurant?.Name ?? "Unknown Restaurant";

            // Combine and sum quantities of identical items, preserving insertion order
            var combinedItems = order.OrderItems.GroupBy(i => i.MenuItemId).Select(g => new
                {
                    Item = g.First().RestaurantMenuItem,
                    TotalQuantity = g.Sum(i => i.Quantity),
                    InsertionIndex = order.OrderItems.IndexOf(g.First()) // To preserve order
                })
                .OrderBy(g => g.InsertionIndex)
                .Select(g => $"{g.TotalQuantity} x {g.Item.Name}");

            string itemLines = string.Join("\n", combinedItems);

            return $"Order #{order.Id} from {restaurantName}: {order.Status}\n{itemLines}\n";
        });

        return statusMessages.ToList();
    }
    public Order CreateDraftOrder(string customerId, string restaurantId)
    {
        var order = new Order()
        {
            Id = (_orders.Count + 1).ToString(),
            CustomerId = customerId,
            RestaurantId = restaurantId,
            Status = OrderStatus.Draft,
            OrderTime = DateTime.Now,
        };
        
        _orders.Add(order);
        return order;
    }

    public int FinaliseOrder(string draftOrderId, Customer customer)
    {
        var draftOrder = _orders.SingleOrDefault(o => o.Id == draftOrderId);
        UpdateOrderStatus(draftOrder, OrderStatus.Ordered);
        
        
        
        // Update Users confirmed orders
        customer.AddOrderToPurchaseHistory(draftOrder);
        return _orders.Count;
    }

    private Order UpdateOrderStatus(Order order, OrderStatus newStatus)
    {
        var validTransitions = new Dictionary<OrderStatus, OrderStatus[]>
        {
            { OrderStatus.Draft, new[] { OrderStatus.Ordered } },
            { OrderStatus.Ordered, new[] { OrderStatus.Accepted, OrderStatus.Rejected, OrderStatus.Cancelled } },
            { OrderStatus.Accepted, new[] { OrderStatus.Cooking, OrderStatus.Cancelled } },
            { OrderStatus.Cooking, new[] { OrderStatus.Cooked } },
            { OrderStatus.Cooked, new[] { OrderStatus.AssignedToDeliverer } },
            { OrderStatus.AssignedToDeliverer, new[] { OrderStatus.PickedUp } },
            { OrderStatus.PickedUp, new[] { OrderStatus.InTransit } },
            { OrderStatus.InTransit, new[] { OrderStatus.Delivered } },
        };

        order.Status = newStatus;

        return order;
    }
    
    public void CancelOrder(string orderId)
    {
        _orders.RemoveAll(o => o.Id == orderId);
    }

    public List<Order> GetAllDeliverableOrders()
    {
        return _orders.Where(x => x.Status == OrderStatus.Cooked).ToList();
    }
}

