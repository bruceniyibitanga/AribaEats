namespace AribaEats.Models;

/// <summary>
/// Represents an individual item within an order (i.e. order line), including its details such as quantity, menu item, and pricing.
/// By seperating the RestaurantMenuItem and the OrderItem allows an order to have different items from different restaurants.
/// </summary>
public class OrderItem
{
    public string Id { get; set; }
    public int OrderId { get; set; }
    public string MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitPrice => RestaurantMenuItem?.Price;
    public Order Order { get; set; }
    public RestaurantMenuItem RestaurantMenuItem { get; set; }
    
    public decimal CalculateSubtotal()
    {
        return (UnitPrice ?? 0) * Quantity;
    }
}