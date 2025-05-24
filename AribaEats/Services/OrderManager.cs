using AribaEats.Models;

namespace AribaEats.Services;

public class OrderManager
{
    private readonly UserManager _userManager;
    private readonly RestaurantManager _restaurantManager;
    private readonly DelivererManager _delivererManager;
    
    public OrderManager(RestaurantManager restaurantManager, UserManager userManager, DelivererManager delivererManager)
    {
        _restaurantManager = restaurantManager;
        _userManager = userManager;
        _delivererManager = delivererManager;
    }
    
    private List<Order> _orders = new();
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

            if (order.Status == OrderStatus.Delivered)
            {
                return
                    $"Order #{order.Id} from {restaurantName}: {order.Status}\nThis order was delivered by {order.Deliverer.Name} (licence plate: {order.Deliverer.LicencePlate})\n{itemLines}\n";
            }

            return $"Order #{order.Id} from {restaurantName}: {order.Status}\n{itemLines}\n";
        });

        return statusMessages.ToList();
    }

    public Order CreateDraftOrder(Customer customer, Restaurant restaurant)
    {
        // Check if item already exits in the Order List. If it exists then simply increase the quanity.
        
        var order = new Order()
        {
            Id = (_orders.Count + 1).ToString(),
            CustomerId = customer.Id,
            RestaurantId = restaurant.Id,
            Status = OrderStatus.Draft,
            OrderTime = DateTime.Now,
            Restaurant = restaurant,
            Customer = customer,
        };
        
        _orders.Add(order);
        return order;
    }

    public int FinaliseOrder(string draftOrderId, Customer customer)
    {
        var draftOrder = _orders.SingleOrDefault(o => o.Id == draftOrderId);
        if (draftOrder != null)
        {
            UpdateOrderToNextStatus(draftOrder);

            // Update Users confirmed orders
            customer.AddOrderToPurchaseHistory(draftOrder);
        }

        return _orders.Count;
    }
    
    public List<Order> GetOrdersReadyForCooking(string restaurantId)
    {
        return _orders.Where(o => o.Status == OrderStatus.Ordered).Where(x => x.RestaurantId == restaurantId).ToList();
    }

    public void UpdateStatusOfOrder(Order order, Restaurant restaurant)
    {
        UpdateOrderToNextStatus(order);
        AddOrderToRestaurantList(restaurant, order);
        _restaurantManager.UpdateRestaurantOrderStatus(restaurant, order);
    }

    public void AddOrderToRestaurantList(Restaurant restaurant, Order order)
    {
        _restaurantManager.AddOrderToRestaurantOrderList(restaurant, order);
    }
    
    private Order UpdateOrderToNextStatus(Order order)
    {
        var transitions = new Dictionary<OrderStatus, OrderStatus>
        {
            { OrderStatus.Draft, OrderStatus.Ordered },
            { OrderStatus.Ordered, OrderStatus.Cooking },
            { OrderStatus.Cooking, OrderStatus.Cooked },
            { OrderStatus.Cooked, OrderStatus.PickedUp },
            { OrderStatus.PickedUp, OrderStatus.Delivered },
        };

        if (transitions.TryGetValue(order.Status, out var nextStatus))
        {
            order.Status = nextStatus;
        }
        else
        {
            throw new InvalidOperationException($"No next status defined for {order.Status}.");
        }

        return order;
    }

    public List<Order> GetCustomerOrderHistory(Customer customer)
    {
        return customer.GetOrderHistory(customer);
    }


    public Dictionary<Deliverer, Order> GetAllAssignedOrders(Restaurant restaurant)
    {
        return _delivererManager.GetAllAssignedOrdersToRestaurant(restaurant);
    }

    public DelivererStatus GetDelivererStatus(Order order)
    {
        return order.Deliverer.Status;
    }
    
    public void CancelOrder(string orderId)
    {
        _orders.RemoveAll(o => o.Id == orderId);
    }

    public List<Order> GetAllDeliverableOrders()
    {
        return _orders.Where(x => x.Status == OrderStatus.Cooked | x.Status == OrderStatus.Ordered).ToList();
    }
}

