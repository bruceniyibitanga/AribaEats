using System.Collections;
using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

/// <summary>
/// Factory class responsible for creating and managing different types of menus for the AribaEats application.
/// Handles menu creation for different user types (Customer, Deliverer, Client) and manages authentication menus.
/// </summary>
public class MenuFactory : IMenuFactory
{
    #region Private Fields
    
    private readonly UserManager _userManager;
    private readonly RestaurantManager _restaurantManager;
    private readonly OrderManager _orderManager;
    private readonly UserDisplayService _userDisplayService;
    private readonly RestaurantMenuFactory _restaurantMenuFactory;
    private readonly DelivererManager _delivererManager;
    private readonly DelivererScreenFactory _delivererScreenFactory;
    
    #endregion

    #region Constructor
    
    /// <summary>
    /// Initialises a new instance of the MenuFactory with required dependencies.
    /// </summary>
    /// <param name="userManager">Manages user operations like login, logout, and registration</param>
    /// <param name="restaurantManager">Manages restaurant-related operations</param>
    /// <param name="orderManager">Handles order management and status tracking</param>
    /// <param name="userDisplayService">Service for displaying user information</param>
    /// <param name="restaurantMenuFactory">Factory for creating restaurant-specific menus</param>
    /// <param name="delivererManager">Manages deliverer operations and availability</param>
    /// <param name="delivererScreenFactory">Factory for creating deliverer-specific screens</param>
    public MenuFactory(
        UserManager userManager, 
        RestaurantManager restaurantManager, 
        OrderManager orderManager, 
        UserDisplayService userDisplayService,
        RestaurantMenuFactory restaurantMenuFactory, 
        DelivererManager delivererManager,
        DelivererScreenFactory delivererScreenFactory)
    {
        _userManager = userManager;
        _restaurantManager = restaurantManager;
        _orderManager = orderManager;
        _userDisplayService = userDisplayService;
        _restaurantMenuFactory = restaurantMenuFactory;
        _delivererManager = delivererManager;
        _delivererScreenFactory = delivererScreenFactory;
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Creates an appropriate menu based on the user type using pattern matching.
    /// </summary>
    /// <param name="user">The user for whom to create the menu</param>
    /// <param name="navigator">Menu navigation controller</param>
    /// <returns>An IMenu instance tailored to the user type</returns>
    /// <exception cref="ArgumentException">Thrown when an unknown user type is provided</exception>
    public IMenu CreateMenuFor(IUser user, MenuNavigator navigator)
    {
        // Use pattern matching to determine user type and create appropriate menu
        return user switch
        {
            Customer customer => CreateMenuForCustomer(navigator, customer),
            Deliverer deliverer => CreateMenuForDeliverer(navigator, deliverer),
            Client client => CreateMenuForClient(navigator, client),
            _ => throw new ArgumentException("Unknown user type")
        };
    }
    
    /// <summary>
    /// Creates the initial login menu without requiring registration menu upfront.
    /// This allows for lazy initialization of the registration menu to avoid circular dependencies.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <returns>Login menu with placeholder for registration option</returns>
    public IMenu GetLoginMenu(MenuNavigator navigator)
    {
        // Create login menu with a placeholder for the registration menu option
        // This prevents circular dependency issues during menu creation
        var loginMenu = new ConsoleMenu(
            "Please make a choice from the menu below:",
            new IMenuItem[]
            {
                new ActionMenuItem("Login as a registered user", () =>
                {
                    // Collect user credentials from console input
                    Console.WriteLine("Email:");
                    string? email = Console.ReadLine();

                    Console.WriteLine("Password:");
                    string? password = Console.ReadLine();

                    // Validate input is not empty or whitespace
                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Email and password must not be empty.");
                        return;
                    }

                    // Attempt to authenticate user
                    var user = _userManager.Login(email, password);

                    // Check if login was successful
                    if (user.ProfileExists && user.PasswordCorrect)
                    {
                        Console.WriteLine($"Welcome back, {user.User!.Name}!");
                        // Navigate to user-specific menu based on their type
                        navigator.NavigateTo(CreateMenuFor(user.User, navigator));
                    }
                    else
                        Console.WriteLine("Invalid email or password.");
                }),
                // Placeholder menu item - will be replaced with actual registration navigation
                new ActionMenuItem("Register as a new user", () => { /* Will be replaced */ }),
                new ActionMenuItem("Exit", () => {Console.WriteLine("Thank you for using Arriba Eats!"); Environment.Exit(0); })
            });

        return loginMenu;
    }
    
    /// <summary>
    /// Creates the user registration menu with options for different user types.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="loginMenu">Reference to login menu for post-registration navigation</param>
    /// <returns>Registration menu with user type selection</returns>
    public IMenu GetRegistrationMenu(MenuNavigator navigator, IMenu loginMenu)
    {
        return new ConsoleMenu(
        "Which type of user would you like to register as?",
        new IMenuItem[]
        {
            new ActionMenuItem("Customer", () =>
            {
                // Use specific registrar for customer registration
                var registrar = new CustomerRegistrar(_userManager);
                var customer = registrar.CollectUserInfo();
                registrar.Register(customer, navigator, loginMenu);
            }),
            new ActionMenuItem("Deliverer", () =>
            {
                // Use specific registrar for deliverer registration
                var registrar = new DelivererRegistrar(_userManager);
                var deliverer = registrar.CollectUserInfo();
                registrar.Register(deliverer, navigator, loginMenu);
            }),
            new ActionMenuItem("Client", () =>
            {
                // Use specific registrar for client (restaurant owner) registration
                var registrar = new ClientRegistrar(_userManager);
                var client = registrar.CollectUserInfo();
                registrar.Register(client, navigator, loginMenu);
                // Add the client's restaurant to the system's restaurant list
                _restaurantManager.AddNewRestaurantToList(client.Restaurant);
            }),
            new BackMenuItem("Return to the previous menu", navigator)
        });
    }
    
    /// <summary>
    /// Links the login menu to the registration menu after both are created.
    /// This resolves the circular dependency issue by updating the placeholder menu item.
    /// </summary>
    /// <param name="loginMenu">The login menu to update</param>
    /// <param name="registrationMenu">The registration menu to link to</param>
    /// <param name="navigator">Menu navigation controller</param>
    public void LinkLoginToRegistration(IMenu loginMenu, IMenu registrationMenu, MenuNavigator navigator)
    {
        if (loginMenu is ConsoleMenu consoleLoginMenu)
        {
            // Replace the placeholder registration menu item (index 1) with actual navigation
            consoleLoginMenu.EditMenuItem(1, new NavigateMenuItem("Register as a new user", registrationMenu, navigator));
        }
    }

    /// <summary>
    /// Creates a screen for adding items to a restaurant's menu.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="client">The client (restaurant owner) adding menu items</param>
    public void CreateAddItemToMenuScreen(MenuNavigator navigator, Client client)
    {
        Console.WriteLine("This is your restaurant's current menu:");
        
        // Display current menu items from the client's restaurant
        var menu = client.GetMenu();
        foreach (var item in menu)
        {
            Console.WriteLine(item);
        }

        // Attempt to add a new menu item and display any resulting message
        var message = _restaurantManager.AddMenuItem(client);
        if (!string.IsNullOrWhiteSpace(message)) Console.WriteLine(message);
    }
    
    #endregion

    #region Private Customer Menu Methods
    
    /// <summary>
    /// Creates the main menu for customer users with all available customer actions.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="customer">The customer user</param>
    /// <returns>Customer-specific main menu</returns>
    private IMenu CreateMenuForCustomer(MenuNavigator navigator, Customer customer)
    {
        var menu = new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                // Show customer profile information
                _userDisplayService.DisplayCustomerInformation(customer);
            }),
            new ActionMenuItem("Select a list of restaurants to order from", () =>
            {
                // Navigate to restaurant selection and ordering interface
                navigator.NavigateTo(_restaurantMenuFactory.CreateRestaurantSortMenu(navigator, customer));
            }),
            new ActionMenuItem("See the status of your orders", () =>
            {
                // Check if customer has any orders before displaying status
                if (customer.GetOrderCount() == 0)
                {
                    Console.WriteLine("You have not placed any orders.");
                }
                else
                {
                    // Retrieve and display all order statuses for this customer
                    var orderStatuses = _orderManager.GetOrderStatuses(customer.Id);
                    foreach (var status in orderStatuses)
                    {
                        Console.WriteLine(status);
                    }
                }
            }),
            new ActionMenuItem("Rate a restaurant you've ordered from", () =>
            {
                // Navigate to restaurant rating interface
                navigator.NavigateTo(CreateRateRestaurantOrderedFromMenu(navigator, customer, _orderManager));
            }),
            new ActionMenuItem("Log out", () =>
            {
                // Perform logout and return to login screen
                _userManager.Logout(customer.Id);
                Console.WriteLine("You are now logged out.");
                    
                // Recreate login and registration menus for next user
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                    
                navigator.NavigateTo(loginMenu);
                navigator.Clear(); // Clear navigation history
            })
        });
        
        // Set this as the home menu for customer navigation
        navigator.SetHomeMenu("customer", menu);
        return menu;
    }
    
    /// <summary>
    /// Creates a menu for customers to rate restaurants from their delivered orders.
    /// Only shows orders that are delivered and haven't been rated yet by this customer.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="customer">The customer who wants to rate restaurants</param>
    /// <param name="orderManager">Order management service</param>
    /// <returns>Menu with rateable orders</returns>
    private IMenu CreateRateRestaurantOrderedFromMenu(MenuNavigator navigator, Customer customer, OrderManager orderManager)
    {
        // Get delivered orders that haven't been rated by this customer yet
        List<Order> previousOrderedList = _orderManager.GetCustomerOrderHistory(customer)
            .Where(order => order.Status == OrderStatus.Delivered) // Only delivered orders
            .Where(order => !order.Rating.Any(r => r.CustomerName == customer.Name)) // Not yet rated by this customer
            .ToList();

        var menuItems = new List<IMenuItem>();
        
        // Create menu items for each rateable order
        for (var i = 0; i < previousOrderedList.Count; i++)
        {
            int capturedIndex = i; // Capture loop variable safely for closure
            menuItems.Add(new ActionMenuItem($"Order #{previousOrderedList[capturedIndex].Id} from {previousOrderedList[capturedIndex].Restaurant.Name}", () =>
            {
                var order = previousOrderedList[capturedIndex];
                Console.WriteLine($"You are rating order #{order.Id} from {order.Restaurant.Name}:");

                // Display order items for context
                foreach (var item in order.OrderItems)
                {
                    Console.WriteLine($"{item.Quantity} x {item.RestaurantMenuItem.Name}");
                }
                
                // Collect rating from user with validation
                int rating = -1;
                while (true)
                {
                    Console.WriteLine("Please enter a rating for this restaurant (1-5, 0 to cancel):");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out rating) && rating >= 0 && rating <= 5)
                        break;

                    Console.WriteLine("Invalid rating.");
                }

                // Handle cancellation
                if (rating == 0)
                {
                    navigator.NavigateBack();
                    return;
                }

                // Collect optional comment
                Console.WriteLine("Please enter a comment to accompany this rating:");
                string comment = Console.ReadLine();

                // Create and save new rating
                var newRating = new Rating
                {
                    Score = rating,
                    CustomerName = customer.Name,
                    Comment = comment,
                    Order = previousOrderedList[capturedIndex],
                    OrderId = previousOrderedList[capturedIndex].Id,
                    Restaurant = previousOrderedList[capturedIndex].Restaurant,
                    RestaurantId = previousOrderedList[capturedIndex].Restaurant.Id
                };
                
                // Add rating to both restaurant and order
                previousOrderedList[capturedIndex].Restaurant.Ratings.Add(newRating);
                if (previousOrderedList[capturedIndex].Id == order.Id) 
                    previousOrderedList[capturedIndex].Rating.Add(newRating);

                Console.WriteLine($"Thank you for rating {previousOrderedList[capturedIndex].Restaurant.Name}.");

                // Return to customer home menu
                navigator.NavigateHome("customer");
            }));
        }
        
        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));
        
        return new ConsoleMenu(
            "Select a previous order to rate the restaurant it came from:",
            menuItems.ToArray()
        );
    }
    
    #endregion

    #region Private Deliverer Menu Methods
    
    /// <summary>
    /// Creates the main menu for deliverer users with all available deliverer actions.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="deliverer">The deliverer user</param>
    /// <returns>Deliverer-specific main menu</returns>
    private IMenu CreateMenuForDeliverer(MenuNavigator navigator, Deliverer deliverer)
    {
        var menu = new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                // Show deliverer profile and current delivery status
                _userDisplayService.DisplayDelivererInformation(deliverer);
                var status = _delivererManager.GetCurrentDeliveryStatus(deliverer);
                if(!string.IsNullOrWhiteSpace(status)) Console.WriteLine(status);
            }),
            new ActionMenuItem("List orders available to deliver", () =>
            {
                // Check if deliverer is available to take new orders
                if (!_delivererManager.IsAvailable(deliverer))
                {
                    Console.WriteLine("You have already selected an order for delivery.");
                    return;
                };
                
                // Show available deliveries if deliverer is free
                navigator.NavigateTo(_delivererScreenFactory.CreateAvailableDeliveriesScreen(navigator, deliverer));
            }),
            new ActionMenuItem("Arrived at restaurant to pick up order", () =>
            {
                // Handle arrival at restaurant for pickup
                _delivererScreenFactory.ShowArrivedAtRestaurantScreen(navigator, deliverer);
            }),
            new ActionMenuItem("Mark this delivery as complete", () =>
            {
                // Complete current delivery
                _delivererScreenFactory.CreateCompleteDeliveryScreen(navigator, deliverer);
            }),
            new ActionMenuItem("Log out", () =>
            {
                // Perform logout and return to login screen
                _userManager.Logout(deliverer.Id);
                Console.WriteLine("You are now logged out.");
                
                // Recreate login and registration menus for next user
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                
                navigator.NavigateTo(loginMenu);
                navigator.Clear(); // Clear navigation history
            })
        });
        
        // Set this as the home menu for deliverer navigation
        navigator.SetHomeMenu("deliverer", menu);
        return menu;
    }
    
    #endregion

    #region Private Client Menu Methods
    
    /// <summary>
    /// Creates the main menu for client users (restaurant owners) with all available restaurant management actions.
    /// </summary>
    /// <param name="navigator">Menu navigation controller</param>
    /// <param name="client">The client (restaurant owner) user</param>
    /// <returns>Client-specific main menu</returns>
    public IMenu CreateMenuForClient(MenuNavigator navigator, Client client)
    {
        var menu = new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                // Show client and restaurant information
                _userDisplayService.DisplayClientInformation(client);
            }),
            new ActionMenuItem("Add item to restaurant menu", () =>
            {
                // Add new items to restaurant menu
                CreateAddItemToMenuScreen(navigator, client);
            }),
            new ActionMenuItem("See current orders", () =>
            {
                // TODO: This function name is misleading - it should be renamed to better reflect its purpose
                // The function should be displaying orders but it actually returns a list of menu items
                _restaurantMenuFactory.ShowCurrentOrderScreen(navigator, client);
            }),
            new ActionMenuItem("Start cooking order", () =>
            {
                // TODO: These functions need a complete refactor! Very messy and don't like the summation function...
                // Navigate to order selection for cooking
                navigator.NavigateTo(_restaurantMenuFactory.CreateSelectOrderToCookScreen(navigator, client));
            }),
            new ActionMenuItem("Finish cooking order", () =>
            {
                // Mark orders as finished cooking
                navigator.NavigateTo(_restaurantMenuFactory.CreateOrderFinishedCookingScreen(navigator, client));
            }),
            new ActionMenuItem("Handle deliverers who have arrived", () =>
            {
                // Manage deliverers arriving for pickup
                navigator.NavigateTo(_restaurantMenuFactory.CreateHandleMultpleDeliverersArrivalScreen(navigator, client));
            }),
            new ActionMenuItem("Log out", () =>
            {
                // Perform logout and return to login screen
                _userManager.Logout(client.Id);
                Console.WriteLine("You are now logged out.");
                
                // Recreate login and registration menus for next user
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                
                navigator.NavigateTo(loginMenu);
                navigator.Clear(); // Clear navigation history
            })
        });
        
        // Set this as the home menu for client navigation
        navigator.SetHomeMenu("client", menu);
        return menu;
    }
    
    #endregion
}