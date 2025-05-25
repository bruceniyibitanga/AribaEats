namespace AribaEats.Models;

/// <summary>
/// Represents the various states an order can go through from creation to delivery or cancellation.
/// </summary>

public enum OrderStatus
{
    Draft,     // Initial state when customer places an order
    Ordered,   // When order has been completed
    Cooking,   // Restaurant has started preparing the food
    Cooked,    // Food is ready for pickup
    PickedUp,  // Deliverer has picked up the order from restaurant
    InTransit, // Food is on the way to customer
    Delivered, // Order has been delivered to customer
}

/// <summary>
/// Represents an order placed by a customer, capturing details such as items, status, and relationships with other entities like customer, restaurant, and deliverer.
/// </summary>

public class Order
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public string RestaurantId { get; set; }
    public string? DelivererId { get; set; }
    public List<Rating> Rating { get; set; } = new();
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public DateTime OrderTime { get; set; }
    public decimal TotalAmount => OrderItems?.Sum(item => item.CalculateSubtotal()) ?? 0;
    
    // Reference and Navigation properties
    public Customer Customer { get; set; }
    public Restaurant Restaurant { get; set; }
    public  Deliverer? Deliverer { get; set; } 
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public void AddItem(RestaurantMenuItem menuItem, int quantity)
    {
        
        OrderItems.Add(new OrderItem
        {
            Id = (OrderItems.Count + 1).ToString(),
            MenuItemId = menuItem.Id,
            RestaurantMenuItem = menuItem,
            Quantity = quantity
        });
    }

}