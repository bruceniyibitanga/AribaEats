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

    public Restaurant()
    {
    }
    public Restaurant(string name, string style, int rating, string id)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Style = style;
        Rating = rating;
    }
}