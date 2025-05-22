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
    public int Rating { get; private set; }
    public Location Location { get; set; }
    public List<RestaurantMenuItem> MenuItems { get; private set; } = new List<RestaurantMenuItem>();
    public List<Order> Orders { get; private set; }

    public Restaurant()
    {
    }
    public Restaurant(string name, string style, int rating, string id, Location location)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Style = style;
        Rating = rating;
        MenuItems = new List<RestaurantMenuItem>();
        Location = location;
    }
}