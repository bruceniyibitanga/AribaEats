namespace AribaEats.Models;

public enum OrderStatus
{
    Draft,
    Ordered,   // Initial state when customer places an order
    Accepted,  // Restaurant has seen and accepted the order
    Cooking,   // Restaurant has started preparing the food
    Cooked,    // Food is ready for pickup
    AssignedToDeliverer, // A deliverer has claimed the order
    PickedUp,  // Deliverer has picked up the order from restaurant
    InTransit, // Food is on the way to customer
    Delivered, // Order has been delivered to customer
    Cancelled, // Order was cancelled
    Rejected   // Restaurant rejected the order
}
public class Order
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public string RestaurantId { get; set; }
    public string? DelivererId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public DateTime OrderTime { get; set; }
    public decimal TotalAmount => OrderItems?.Sum(item => item.CalculateSubtotal()) ?? 0;

    
    // Navigation properties
    public Customer Customer { get; set; }
    // public Restaurant Restaurant { get; set; }
    public Deliverer Deliverer { get; set; }
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