namespace AribaEats.Models;

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