namespace AribaEats.Models;

/// <summary>
/// Represents a rating provided by a customer for an order or a restaurant.
/// </summary>
public class Rating
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Score { get; set; }
    public string CustomerName { get; set; }
    public string Comment { get; set; }
    public string OrderId { get; set; }
    public Order Order { get; set; }
    public string RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
}
