using AribaEats.Models;

namespace AribaEats.Services;

/// <summary>
/// Manages deliverers, their assignments, and delivery-related operations.
/// </summary>
public class DelivererManager
{
    /// <summary>
    /// Tracks orders assigned to deliverers.
    /// </summary>
    private Dictionary<Deliverer, Order> _assignedOrders = new Dictionary<Deliverer, Order>();

    /// <summary>
    /// Assigns a deliverer to an order, marks the deliverer in use, and updates the order with deliverer details.
    /// </summary>
    /// <param name="deliverer">The deliverer assigned to the order.</param>
    /// <param name="order">The order being assigned to the deliverer.</param>
    /// <returns>True if the delivery is successfully accepted.</returns>
    public bool AcceptDelivery(Deliverer deliverer, Order order)
    {
        order.Deliverer = deliverer;
        order.DelivererId = deliverer.Id;
        _assignedOrders.Add(deliverer, order);
        return true;
    }

    /// <summary>
    /// Updates the current status of the deliverer.
    /// </summary>
    /// <param name="deliverer">The deliverer whose status is being updated.</param>
    public void UpdateDelivererStatus(Deliverer deliverer)
    {
        deliverer.UpdateDelivererStatus(deliverer);
    }

    /// <summary>
    /// Provides the current delivery status of a deliverer, including order and destination details.
    /// </summary>
    /// <param name="deliverer">The deliverer whose current delivery status is requested.</param>
    /// <returns>A string summarising the deliverer's current delivery, or an empty string if no delivery is assigned.</returns>
    public string GetCurrentDeliveryStatus(Deliverer deliverer)
    {
        if (!_assignedOrders.TryGetValue(deliverer, out var order)) return "";

        var restaurant = order.Restaurant;
        var customer = order.Customer;

        return $"Current delivery:\n" +
               $"Order #{order.Id} from {restaurant.Name} at {restaurant.Location.X},{restaurant.Location.Y}.\n" +
               $"To be delivered to {customer.Name} at {customer.Location.X},{customer.Location.Y}.";
    }

    /// <summary>
    /// Retrieves all orders assigned to deliverers for a specific restaurant.
    /// </summary>
    /// <param name="restaurant">The restaurant for which assigned orders are being queried.</param>
    /// <returns>A dictionary mapping deliverers to their assigned orders for the given restaurant.</returns>
    public Dictionary<Deliverer, Order> GetAllAssignedOrdersToRestaurant(Restaurant restaurant)
    {
        return _assignedOrders
            .Where(entry => entry.Value.Restaurant == restaurant)
            .ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    /// <summary>
    /// Retrieves all orders currently assigned to a specific deliverer.
    /// </summary>
    /// <param name="deliverer">The deliverer whose assigned orders are being queried.</param>
    /// <returns>A list of orders assigned to the deliverer.</returns>
    public List<Order> GetAllOrdersAssignedToDriver(Deliverer deliverer)
    {
        if (_assignedOrders.TryGetValue(deliverer, out var order))
        {
            return new List<Order> { order };
        }
        return new List<Order>();
    }

    /// <summary>
    /// Checks if a deliverer is available to take new deliveries.
    /// </summary>
    /// <param name="deliverer">The deliverer whose availability is being checked.</param>
    /// <returns>True if the deliverer is available, otherwise false.</returns>
    public bool IsAvailable(Deliverer deliverer)
    {
        return !_assignedOrders.ContainsKey(deliverer);
    }

    /// <summary>
    /// Removes an order assignment from a deliverer and marks the deliverer as available for new deliveries.
    /// </summary>
    /// <param name="deliverer">The deliverer whose order assignment is being removed.</param>
    public void RemoveOrderAssignment(Deliverer deliverer)
    {
        _assignedOrders.Remove(deliverer);

        // Mark the deliverer as available again after the delivery is complete.
        deliverer.UpdateDelivererStatus(deliverer);
    }
}