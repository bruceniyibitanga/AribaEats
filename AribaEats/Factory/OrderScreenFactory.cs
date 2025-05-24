using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

public class OrderScreenFactory 
{
    private readonly OrderManager _orderManager;
    private readonly RestaurantManager _restaurantManager;
    public OrderScreenFactory(OrderManager orderManager, RestaurantManager restaurantManager)
    {
        _orderManager = orderManager;
        _restaurantManager = restaurantManager;
    }
    
    // Procesing Order Screen - Allows to add menu items to their order or cancel
    public IMenu CreateOrderProcessingScreen(MenuNavigator navigator, Customer customer, Restaurant restaurant)
    {
        if (!SessionState.HasVisitedRestaurant(customer.Id, restaurant.Id))
        {
            Console.WriteLine($"Placing order from {restaurant.Name}.");
            SessionState.MarkRestaurantVisited(customer.Id, restaurant.Id);
        }

        navigator.SetAnchor();

        return new ConsoleMenu("", new IMenuItem[]
        {
            new ActionMenuItem("See this restaurant's menu and place an order", () =>
            {
                navigator.NavigateTo(CreateRestarantMenuViewScreen(navigator, customer, restaurant));
            }),
            new ActionMenuItem("See reviews for this restaurant", () => { }),
            new ActionMenuItem("Return to main menu", () => { navigator.NavigateHome("customer"); }),
        });
    }


    public IMenu CreateRestarantMenuViewScreen(MenuNavigator navigator, Customer customer, Restaurant restaurant)
    {
        var draftOrder = _orderManager.CreateDraftOrder(customer, restaurant);

        Console.WriteLine($"Current order total: ${draftOrder.TotalAmount:F2}");

        var screenMenuItems = new List<IMenuItem>();

        foreach (var item in restaurant.MenuItems)
        {
            screenMenuItems.Add(new ActionMenuItem(item.ToString(), () =>
            {
                ExecuteWithTotalUpdate(() =>
                {
                    Console.WriteLine($"Adding {item.Name} to order.");
                    var quantity = TryGetValidQuantity();
                    if (quantity == 0)
                    {
                        return;
                    }

                    draftOrder.AddItem(item, quantity);
                    Console.WriteLine($"Added {quantity} x {item.Name} to order.");
                }, draftOrder);
            }));
        }

        screenMenuItems.Add(new ActionMenuItem("Complete order", () =>
        {
            if (draftOrder.OrderItems.Count == 0)
            {
                Console.WriteLine("You must add at least one item before completing the order.");
                return;
            }

            var finalisedOrder = _orderManager.FinaliseOrder(draftOrder.Id, customer);
            Console.WriteLine($"Your order has been placed. Your order number is #{finalisedOrder}.");
            navigator.NavigateToAnchor();
        }));

        screenMenuItems.Add(new ActionMenuItem("Cancel order", () =>
        {
            _orderManager.CancelOrder(draftOrder.Id);
            navigator.NavigateToAnchor();
        }));

        return new ConsoleMenu("", screenMenuItems.ToArray());
    }
    
    private int TryGetValidQuantity()
    {
        while (true)
        {
            Console.WriteLine("Please enter quantity (0 to cancel):");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int quantity) && quantity >= 0)
            {
                return quantity;
            }

            Console.WriteLine("Invalid input.");
        }
    }
    
    // Status of Order
    
    
    // Rate Restaurant customer has ordered from
    
    private void ExecuteWithTotalUpdate(Action action, Order draftOrder)
    {
        action.Invoke();
        Console.WriteLine($"Current order total: ${draftOrder.TotalAmount:F2}");
    }
}