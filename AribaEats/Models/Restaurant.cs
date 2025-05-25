namespace AribaEats.Models;

public enum FoodStyle
{
    
}
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
    public void AddOrderToList(Order order)
    {
        if(order.Restaurant.Id == Id) Orders.Add(order);
    }

    public List<Order> GetAllOrdersThatAreCooking()
    {
        return Orders.Where(x => x.Status == OrderStatus.Cooking).ToList();
    }

    public List<Order> GetAllorderThatAreReadyForDelivery(Restaurant restaurant)
    {
        return Orders.Where(x => x.Status == OrderStatus.Cooked).ToList();
    }

    public void UpdateOrderStatus(Order order)
    {
        var currentOrder = Orders.FirstOrDefault(x => x.Id == order.Id);
        if (currentOrder != null) currentOrder.Status = order.Status;
    }
}