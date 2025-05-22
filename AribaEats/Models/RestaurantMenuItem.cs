namespace AribaEats.Models;

public class RestaurantMenuItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public decimal Price { get; set; }

    public RestaurantMenuItem()
    {
        
    }
    public RestaurantMenuItem(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
    public override string ToString()
    {
        return $"{Price,7:$0.00}  {Name}";
    }
}