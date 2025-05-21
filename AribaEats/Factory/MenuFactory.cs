using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Factory;

public class MenuFactory : IMenuFactory
{
    private readonly UserManager _userManager;
    private readonly RestaurantManager _restaurantManager;
    private readonly OrderManager _orderManager;
    private readonly UserDisplayService _userDisplayService;
    
    public MenuFactory(UserManager userManager, RestaurantManager restaurantManager, OrderManager orderManager, UserDisplayService userDisplayService)
    {
        _userManager = userManager;
        _restaurantManager = restaurantManager;
        _orderManager = orderManager;
        _userDisplayService = userDisplayService;
    }
    
    public IMenu CreateMenuFor(IUser user, MenuNavigator navigator)
    {
        return user switch
        {
            Customer customer => CreateMenuForCustomer(navigator, customer),
            Deliverer deliverer => CreateMenuForDeliverer(navigator, deliverer),
            Client client => CreateMenuForClient(navigator, client),
            _ => throw new ArgumentException("Unknown user type")
        };
    }
    
    // Create login menu without requiring registration menu upfront
    public IMenu GetLoginMenu(MenuNavigator navigator)
    {
        // Create login menu with a placeholder for the registration menu option
        var loginMenu = new ConsoleMenu(
            "Please make a choice from the menu below:",
            new IMenuItem[]
            {
                new ActionMenuItem("Login as a registered user", () =>
                {
                    Console.WriteLine("Email:");
                    string? email = Console.ReadLine();

                    Console.WriteLine("Password:");
                    string? password = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Email and password must not be empty.");
                        return;
                    }

                    var user = _userManager.Login(email, password);

                    if (user.ProfileExists && user.PasswordCorrect)
                    {
                        Console.WriteLine($"Welcome back, {user.User!.Name}!");
                        navigator.NavigateTo(CreateMenuFor(user.User, navigator));
                    }
                    else
                        Console.WriteLine("Invalid email or password.");
                }),
                // Placeholder for registration menu link - will be updated later
                new ActionMenuItem("Register as a new user", () => { /* Will be replaced */ }),
                new ActionMenuItem("Exit", () => {Console.WriteLine("Thank you for using Arriba Eats!"); Environment.Exit(0); })
            });

        return loginMenu;
    }
    
    // Create the registration menu
    public IMenu GetRegistrationMenu(MenuNavigator navigator, IMenu loginMenu)
    {
        return new ConsoleMenu(
            "Which type of user would you like to register as?",
            new IMenuItem[]
            {
                new ActionMenuItem("Customer", () =>
                {
                    var registrar = new CustomerRegistrar(_userManager);
                    var customer = registrar.CollectUserInfo();
                    registrar.Register(customer, navigator, loginMenu);
                }),
                new ActionMenuItem("Deliverer", () =>
                {
                    var registrar = new DelivererRegistrar(_userManager);
                    var deliverer = registrar.CollectUserInfo();
                    registrar.Register(deliverer, navigator, loginMenu);
                }),
                new ActionMenuItem("Client", () =>
                {
                    var registrar = new ClientRegistrar(_userManager);
                    var client = registrar.CollectUserInfo();
                    registrar.Register(client, navigator, loginMenu);
                }),
                new BackMenuItem("Return to the previous menu", navigator)
            });
    }
    
    // Link login menu to registration menu after both are created
    public void LinkLoginToRegistration(IMenu loginMenu, IMenu registrationMenu, MenuNavigator navigator)
    {
        if (loginMenu is ConsoleMenu consoleLoginMenu)
        {
            // Replace the placeholder registration menu item with actual navigation
            consoleLoginMenu.EditMenuItem(1, new NavigateMenuItem("Register as a new user", registrationMenu, navigator));
        }
    }
    
    // CUSTOMER MENUS
    private IMenu CreateMenuForCustomer(MenuNavigator navigator, Customer customer)
    {
        return new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                _userDisplayService.DisplayCustomerInformation(customer);
            }),
            new ActionMenuItem("Select a list of restaurants to order from", () =>
            {
                // When a user selects this will navigate to the restaurant sort menu first
                navigator.NavigateTo(CreateRestrauntSortMenu(navigator, customer, _restaurantManager));
            }),
            new ActionMenuItem("See the status of your orders", () =>
            {
                if (customer.GetOrderCount() == 0) Console.WriteLine("You have not placed any orders.");
                // TODO: REALLY THINK ABOUT THE ORDER FUNCTIONALITY AND WHETHER THERE SHOULD BE AN ORDER MANAGER?!
            }),
            new ActionMenuItem("Rate a restaurant you've ordered from", () =>
            {
                navigator.NavigateTo(CreateRestaurantOrderedFromMenu(navigator, customer, _orderManager));
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout();
                Console.WriteLine("You are now logged out.");
                
                // If a user has logged out. We want to clear the navigation history. And then redirect them to the login page.
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                
                navigator.NavigateTo(loginMenu);
                navigator.Clear();
            })
        });
    }
    
    private IMenu CreateRestrauntSortMenu(MenuNavigator navigator, Customer customer, RestaurantManager restaurantManager)
    {
        return new ConsoleMenu("How would you like the list of restaurants ordered?", new IMenuItem[]
        {
            new ActionMenuItem("Sorted alphabetically by name", () => navigator.NavigateTo(CreateRestrauntListMenu(navigator, customer, restaurantManager, "alphabetically"))),
            new ActionMenuItem("Sorted by distance", () => navigator.NavigateTo(CreateRestrauntListMenu(navigator, customer, restaurantManager, "distance"))),
            new ActionMenuItem("Sorted by style", () => navigator.NavigateTo(CreateRestrauntListMenu(navigator, customer, restaurantManager, "style"))),
            new ActionMenuItem("Sorted by average rating", () => navigator.NavigateTo(CreateRestrauntListMenu(navigator, customer, restaurantManager, "rating"))),
            new BackMenuItem("Return to the previous menu", navigator)
        });
    }

    private IMenu CreateRestrauntListMenu(MenuNavigator navigator, Customer user, RestaurantManager restaurantManager, string sortOrder)
    {
        // Fixed the condition below - it was incorrectly structured
        if (user.GetOrderCount() == 0)
        {
            Console.WriteLine("You have not placed any orders.");
            return CreateMenuForCustomer(navigator, user);
        }
        
        // TODO: Implement restaurant listing based on sort order
        var menuItems = new List<IMenuItem>
        {
            // Add restaurant items here based on sortOrder
            new BackMenuItem("Return to the previous menu", navigator)
        };

        return new ConsoleMenu("You can order from the following restaurants:", menuItems.ToArray());
    }

    private IMenu CreateRestaurantOrderedFromMenu(MenuNavigator navigator, Customer customer, OrderManager orderManager)
    {
        List<string> orderedOrderStatuses = orderManager.GetOrderStatuses(customer.Id);

        var menuItems = new List<IMenuItem>();

        foreach (var orderStatus in orderedOrderStatuses)
        {
            menuItems.Add(new ActionMenuItem(orderStatus, () =>
            {
                // TODO: Implement rating functionality
            }));
        }

        menuItems.Add(new BackMenuItem("Return to the previous menu", navigator));
        
        return new ConsoleMenu(
            "Select a previous order to rate the restaurant it came from:",
            menuItems.ToArray()
        );
    }
    
    // DELIVERER
    private IMenu CreateMenuForDeliverer(MenuNavigator navigator, Deliverer deliverer)
    {
        return new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                _userDisplayService.DisplayDelivererInformation(deliverer);
            }),
            new ActionMenuItem("List orders available to deliver", () =>
            {
                // TODO: Implement order listing for deliverer
            }),
            new ActionMenuItem("Arrived at restaurant to pick up order", () =>
            {
                // TODO: Implement order pickup
            }),
            new ActionMenuItem("Mark this delivery as complete", () =>
            {
                // TODO: Implement order completion
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout();
                Console.WriteLine("You are now logged out.");
                
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                
                navigator.NavigateTo(loginMenu);
                navigator.Clear();
            })
        });
    }
    
    // CLIENT
    public IMenu CreateMenuForClient(MenuNavigator navigator, Client client)
    {
        return new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                _userDisplayService.DisplayClientInformation(client);
            }),
            new ActionMenuItem("Add item to restaurant menu", () =>
            {
                // TODO: Implement order listing for client
            }),
            new ActionMenuItem("See current orders", () =>
            {
                // TODO: Implement restaurant-related functionality
            }),
            new ActionMenuItem("Start cooking order", () =>
            {
                // TODO: Implement order completion for client
            }),
            new ActionMenuItem("Finish cooking order", () => {}),
            new ActionMenuItem("Handle deliverers who have arrived", () => {}),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout();
                Console.WriteLine("You are now logged out.");
                
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                
                navigator.NavigateTo(loginMenu);
                navigator.Clear();
            })
        });
    }
}