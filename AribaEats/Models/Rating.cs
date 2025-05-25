namespace AribaEats.Models;

public class Rating
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Score { get; set; }
    public string CustomerName { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string OrderId { get; set; }
    public Order Order { get; set; }

    public string RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
}
