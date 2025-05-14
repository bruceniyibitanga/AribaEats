namespace AribaEats.Models;

public enum FoodStyle
{
    
}
public class Restaurant
{
    public string Id { get; private set; }
    public string Name { get; set; }
    public Location Location { get; private set; }
    public int Rating { get; private set; }

    public Restaurant(string name, Location location)
    {
        Name = name;
        Location = location;
    }
}