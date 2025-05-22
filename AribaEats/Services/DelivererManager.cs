namespace AribaEats.Services;

public class DelivererManager
{
    private readonly OrderManager _orderManager;
    private readonly RestaurantManager _restaurantManager;

    public DelivererManager(OrderManager orderManager, RestaurantManager restaurantManager)
    {
        _orderManager = orderManager;
        _restaurantManager = restaurantManager;
    }
    
    
}