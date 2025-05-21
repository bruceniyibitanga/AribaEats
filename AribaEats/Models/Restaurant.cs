namespace AribaEats.Models;

public enum FoodStyle
{
    
}
public class Restaurant
{
    public string Id { get; private set; }
    public string Name { get; set; }
    public string Style { get; set; }
    public int Rating { get; private set; }
    
    public Restaurant(string name, string style, int rating)
    {
        Name = name;
        Rating = rating;
        Style = style;
    }
}