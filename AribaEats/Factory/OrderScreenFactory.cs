using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

/// <summary>
/// Factory class to create order-related menu screens for customers.
/// </summary>
public class OrderScreenFactory 
{
    private readonly OrderManager _orderManager;
    private readonly RestaurantManager _restaurantManager;

    /// <summary>
    /// Initialises a new instance of OrderScreenFactory.
    /// </summary>
    /// <param name="orderManager">OrderManager instance to manage orders.</param>
    /// <param name="restaurantManager">RestaurantManager instance to manage restaurants.</param>
    public OrderScreenFactory(OrderManager orderManager, RestaurantManager restaurantManager)
    {
        _orderManager = orderManager;
        _restaurantManager = restaurantManager;
    }

    /// <summary>
    /// Creates the order processing screen menu, allowing a customer to view menu, place orders, or see reviews.
    /// </summary>
    /// <param name="navigator">Menu navigation controller.</param>
    /// <param name="customer">The customer placing the order.</param>
    /// <param name="restaurant">The restaurant the customer is ordering from.</param>
    /// <returns>A menu interface for order processing.</returns>
    public IMenu CreateOrderProcessingScreen(MenuNavigator navigator, Customer customer, Restaurant restaurant)
    {
        // If the customer hasn't visited this restaurant before, print a welcome message
        if (!SessionState.HasVisitedRestaurant)
        {
            Console.WriteLine($"Placing order from {restaurant.Name}.");
        }

        // Save the current navigation position so the user can return here later
        navigator.SetAnchor();

        // Create and return a menu with three options
        return new ConsoleMenu("", new IMenuItem[]
        {
            // Option 1: Show the restaurant's menu to allow placing an order
            new ActionMenuItem("See this restaurant's menu and place an order", () =>
            {
                // Navigate to the screen where the customer can view the restaurant menu
                navigator.NavigateTo(CreateRestarantMenuViewScreen(navigator, customer, restaurant));
            }),

            // Option 2: Show reviews for the restaurant
            new ActionMenuItem("See reviews for this restaurant", () =>
            {
                // Retrieve reviews for the specified restaurant
                var reviews = _restaurantManager.GetRestaurantReviews(restaurant);

                // Variable siglePrintStatment controls how many times the rating is printed (currently always 1)
                int siglePrintStatment = 1;

                // Loop through each review and display details
                foreach (var review in reviews)
                {
                    // Print the reviewer's name
                    Console.Write($"Reviewer: {review.CustomerName}");

                    // Loop to print rating lines (only once here)
                    for (int i = 0; i < siglePrintStatment; i++)
                    {
                        Console.Write($"\nRating: ");

                        // Print stars (*) representing the review score
                        for (int j = 0; j < review.Score; j++)
                        {
                            Console.Write("*");
                        }

                        Console.Write("\n"); // New line after stars
                    }

                    // Print the review comment
                    Console.WriteLine($"Comment: {review.Comment}\n");
                }
            }),

            // Option 3: Return to the main menu (customer home screen)
            new ActionMenuItem("Return to main menu", () => { navigator.NavigateHome("customer"); }),
        });
    }

    /// <summary>
    /// Creates the restaurant menu view screen where a customer can add items to a draft order.
    /// </summary>
    /// <param name="navigator">Menu navigator controller.</param>
    /// <param name="customer">Customer placing the order.</param>
    /// <param name="restaurant">Restaurant from which to order.</param>
    /// <returns>A menu interface to view and modify the draft order.</returns>
    public IMenu CreateRestarantMenuViewScreen(MenuNavigator navigator, Customer customer, Restaurant restaurant)
    {
        var draftOrder = _orderManager.CreateDraftOrder(customer, restaurant);

        Console.WriteLine($"Current order total: ${draftOrder.TotalAmount:F2}");

        var screenMenuItems = new List<IMenuItem>();

        foreach (var item in restaurant.MenuItems)
        {
            screenMenuItems.Add(new ActionMenuItem(item.ToString(), () =>
            {
                // This little helper function (ExecuteWithTotalUpdate) allows
                // us to execute the method and rerender the menu again
                // By doing this it allows us to simulate dynamic updating of the Total amount.
                ExecuteWithTotalUpdate(() =>
                {
                    Console.WriteLine($"Adding {item.Name} to order.");
                    var quantity = TryGetValidQuantity();
                    if (quantity == 0) return;

                    draftOrder.AddItem(item, quantity);
                    Console.WriteLine($"Added {quantity} x {item.Name} to order.");
                }, draftOrder);
            }));
        }
        
        // Add menu items to a list to be shown on screen when return the Menu.
        screenMenuItems.Add(new ActionMenuItem("Complete order", () =>
        {
            if (draftOrder.OrderItems.Count == 0)
            {
                Console.WriteLine("You must add at least one item before completing the order.");
                return;
            }

            var finalisedOrder = _orderManager.FinaliseOrder(draftOrder.Id, customer);
            Console.WriteLine($"Your order has been placed. Your order number is #{finalisedOrder}.");
            navigator.NavigateToAnchor(); // Return to previous menu point
        }));

        screenMenuItems.Add(new ActionMenuItem("Cancel order", () =>
        {
            _orderManager.CancelOrder(draftOrder.Id);
            navigator.NavigateToAnchor(); // Return to previous menu point
        }));

        return new ConsoleMenu("", screenMenuItems.ToArray());
    }

    /// <summary>
    /// Helper method to prompt the user to enter a valid quantity for an order item.
    /// </summary>
    /// <returns>The quantity entered by the user, or 0 if canceled.</returns>
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

    /// <summary>
    /// Executes an action and then displays the updated total amount of the draft order.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="draftOrder">The draft order to display total for.</param>
    private void ExecuteWithTotalUpdate(Action action, Order draftOrder)
    {
        action.Invoke();
        Console.WriteLine($"Current order total: ${draftOrder.TotalAmount:F2}");
    }
}