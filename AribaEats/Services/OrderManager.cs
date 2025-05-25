using AribaEats.Models;

namespace AribaEats.Services;

/// <summary>
/// Manages orders, handles their lifecycle, and interacts with related managers such as restaurant, user, and deliverer managers.
/// </summary>
public class OrderManager
{
    // Manages user-related operations.
    private readonly UserManager _userManager; 
    // Manages restaurant-related operations.
    private readonly RestaurantManager _restaurantManager; 
    // Manages deliverer-related operations.
    private readonly DelivererManager _delivererManager; 

    private List<Order> _orders = new(); // Internal list of all orders in the system.

    /// <summary>
    /// Initialises an instance of the <see cref="OrderManager"/> class.
    /// </summary>
    /// <param name="restaurantManager">Manages restaurant information and operations.</param>
    /// <param name="userManager">Manages customer-related information and operations.</param>
    /// <param name="delivererManager">Manages deliverer-related information and operations.</param>
    public OrderManager(RestaurantManager restaurantManager, UserManager userManager, DelivererManager delivererManager)
    {
        _restaurantManager = restaurantManager;
        _userManager = userManager;
        _delivererManager = delivererManager;
    }

    /// <summary>
    /// Retrieves the status of all orders placed by a specified customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer whose order statuses are to be retrieved.</param>
    /// <returns>A list of order status strings.</returns>
    public List<string> GetOrderStatuses(string customerId)
    {
        var customerOrders = _orders.Where(o => o.CustomerId == customerId).ToList();

        var statusMessages = customerOrders.Select(order =>
        {
            var restaurant = _restaurantManager.GetRestaurantById(order.RestaurantId);
            var restaurantName = restaurant?.Name ?? "Unknown Restaurant";

            // Combine and sum quantities of identical items, preserving order
            var combinedItems = order.OrderItems.GroupBy(i => i.MenuItemId).Select(g => new
                {
                    Item = g.First().RestaurantMenuItem,
                    TotalQuantity = g.Sum(i => i.Quantity),
                    InsertionIndex = order.OrderItems.IndexOf(g.First())
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

    /// <summary>
    /// Creates a draft order for a customer at a specific restaurant.
    /// Will only be approved as an order after use complete the order.
    /// </summary>
    /// <param name="customer">The customer placing the order.</param>
    /// <param name="restaurant">The restaurant for which the order is created.</param>
    /// <returns>The newly created draft order.</returns>
    public Order CreateDraftOrder(Customer customer, Restaurant restaurant)
    {
        var order = new Order
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

    /// <summary>
    /// Finalises a draft order, transitions it to the next status, and adds it to the customer's purchase history.
    /// </summary>
    /// <param name="draftOrderId">The ID of the draft order to finalize.</param>
    /// <param name="customer">The customer associated with the order.</param>
    /// <returns>The total number of orders.</returns>
    public int FinaliseOrder(string draftOrderId, Customer customer)
    {
        var draftOrder = _orders.SingleOrDefault(o => o.Id == draftOrderId);
        if (draftOrder != null)
        {
            UpdateOrderToNextStatus(draftOrder);

            // Add order to the customer's purchase history.
            customer.AddOrderToPurchaseHistory(draftOrder);
        }

        return _orders.Count;
    }

    /// <summary>
    /// Retrieves all orders that are ready to be cooked for a specific restaurant.
    /// </summary>
    /// <param name="restaurantId">The ID of the restaurant.</param>
    /// <returns>A list of orders ready to be cooked.</returns>
    public List<Order> GetOrdersReadyForCooking(string restaurantId)
    {
        return _orders.Where(o => o.Status == OrderStatus.Ordered && o.RestaurantId == restaurantId).ToList();
    }

    /// <summary>
    /// Updates the status of an order and ensures it is managed properly within a restaurant.
    /// </summary>
    /// <param name="order">The order to update.</param>
    /// <param name="restaurant">The restaurant associated with the order.</param>
    public void UpdateStatusOfOrder(Order order, Restaurant restaurant)
    {
        UpdateOrderToNextStatus(order);
        AddOrderToRestaurantList(restaurant, order);
        _restaurantManager.UpdateRestaurantOrderStatus(restaurant, order);
    }

    /// <summary>
    /// Adds an order to the internal list of a specific restaurant.
    /// </summary>
    /// <param name="restaurant">The restaurant receiving the order.</param>
    /// <param name="order">The order being added.</param>
    public void AddOrderToRestaurantList(Restaurant restaurant, Order order)
    {
        _restaurantManager.AddOrderToRestaurantOrderList(restaurant, order);
    }

    /// <summary>
    /// Updates an order to its next logical status based on a predefined transition workflow.
    /// </summary>
    /// <param name="order">The order whose status is being updated.</param>
    /// <returns>The updated order.</returns>
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

    /// <summary>
    /// Retrieves a customer's order history.
    /// </summary>
    /// <param name="customer">The customer whose order history is being retrieved.</param>
    /// <returns>A list of the customer's previous orders.</returns>
    public List<Order> GetCustomerOrderHistory(Customer customer)
    {
        return customer.GetOrderHistory(customer);
    }

    /// <summary>
    /// Retrieves all orders assigned to deliverers for a specific restaurant.
    /// </summary>
    /// <param name="restaurant">The restaurant for which orders are being retrieved.</param>
    /// <returns>A dictionary mapping deliverers to their assigned orders.</returns>
    public Dictionary<Deliverer, Order> GetAllAssignedOrders(Restaurant restaurant)
    {
        return _delivererManager.GetAllAssignedOrdersToRestaurant(restaurant);
    }

    /// <summary>
    /// Cancels an order and removes it from the system.
    /// </summary>
    /// <param name="orderId">The ID of the order to cancel.</param>
    public void CancelOrder(string orderId)
    {
        _orders.RemoveAll(o => o.Id == orderId);
    }

    /// <summary>
    /// Retrieves all orders that are ready to be delivered.
    /// </summary>
    /// <returns>A list of orders that can be assigned to deliverers.</returns>
    public List<Order> GetAllDeliverableOrders()
    {
        return _orders
            .Where(x =>
                (x.Status == OrderStatus.Cooked || x.Status == OrderStatus.Ordered)
                && x.Deliverer == null
            )
            .ToList();
    }
}