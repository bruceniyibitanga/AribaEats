namespace AribaEats.Models;

/// <summary>
/// Represents a restaurant with its details such as name, style, location, menu items, orders, and ratings.
/// </summary>
public class Restaurant
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string ClientId { get; set; }
    public string Name { get; set; }
    public string Style { get; set; }
    public double? Rating => Ratings.Count > 0 ? Ratings.Average(r => r.Score) : null;
    public List<Rating> Ratings { get; set; } = new();
    public Location Location { get; set; }
    public List<RestaurantMenuItem> MenuItems { get; private set; } = new List<RestaurantMenuItem>();
    public List<Order> Orders { get; private set; } = new List<Order>();

    public Restaurant()
    {
    }
    public Restaurant(string name, string style, Location location)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Style = style;
        MenuItems = new List<RestaurantMenuItem>();
        Location = location;
    }
    
    /// <summary>
    /// Adds an order to the restaurant's order list if it belongs to this restaurant.
    /// </summary>
    /// <param name="order">The order to add to the list.</param>
    public void AddOrderToList(Order order)
    {
        if(order.Restaurant.Id == Id) Orders.Add(order);
    }

    /// <summary>
    /// Retrieves all orders currently in the "Cooking" status.
    /// </summary>
    /// <returns>A list of orders with the status "Cooking".</returns>
    public List<Order> GetAllOrdersThatAreCooking()
    {
        return Orders.Where(x => x.Status == OrderStatus.Cooking).ToList();
    }
    
    /// <summary>
    /// Updates the status of a given order within the restaurant's order list.
    /// </summary>
    /// <param name="order">The order whose status is to be updated.</param>
    public void UpdateOrderStatus(Order order)
    {
        var currentOrder = Orders.FirstOrDefault(x => x.Id == order.Id);
        if (currentOrder != null) currentOrder.Status = order.Status;
    }
}