using System.Collections;
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
    private readonly RestaurantMenuFactory _restaurantMenuFactory;
    private readonly DelivererManager _delivererManager;
    private readonly DelivererScreenFactory _delivererScreenFactory;
        
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
                _restaurantManager.AddNewRestaurantToList(client.Restaurant);
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
        var menu = new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                _userDisplayService.DisplayCustomerInformation(customer);
            }),
            new ActionMenuItem("Select a list of restaurants to order from", () =>
            {
                navigator.NavigateTo(_restaurantMenuFactory.CreateRestaurantSortMenu(navigator, customer));
            }),
            new ActionMenuItem("See the status of your orders", () =>
            {
                if (customer.GetOrderCount() == 0)
                {
                    Console.WriteLine("You have not placed any orders.");
                }
                else
                {
                    var orderStatuses = _orderManager.GetOrderStatuses(customer.Id);
                    foreach (var status in orderStatuses)
                    {
                        Console.WriteLine(status);
                    }
                }

            }),
            new ActionMenuItem("Rate a restaurant you've ordered from", () =>
            {
                navigator.NavigateTo(CreateRateRestaurantOrderedFromMenu(navigator, customer, _orderManager));
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout(customer.Id);
                Console.WriteLine("You are now logged out.");
                    
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                    
                navigator.NavigateTo(loginMenu);
                navigator.Clear();
            })
        });
        
        navigator.SetHomeMenu("customer", menu);
        return menu;
    }
    
    private IMenu CreateRateRestaurantOrderedFromMenu(MenuNavigator navigator, Customer customer, OrderManager orderManager)
    {
        List<Order> previousOrderedList = _orderManager.GetCustomerOrderHistory(customer);
    
        var menuItems = new List<IMenuItem>();
    
        foreach (var item in previousOrderedList)
        {
            menuItems.Add(new ActionMenuItem($"Order #{item.Id} from {item.Restaurant.Name}", () =>
            {
                Console.WriteLine($"You are rating order #{item.Id} from {item.Restaurant.Name}");
            }));
            foreach (var food in item.OrderItems)
            {
                Console.WriteLine($"{food.Quantity} x {food.RestaurantMenuItem.Name}");
            }
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
        var menu = new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                _userDisplayService.DisplayDelivererInformation(deliverer);
                var status = _delivererManager.GetCurrentDeliveryStatus(deliverer);
                if(!string.IsNullOrWhiteSpace(status))Console.WriteLine(status);
            }),
            new ActionMenuItem("List orders available to deliver", () =>
            {
                if (!_delivererManager.IsAvailable(deliverer))
                {
                    Console.WriteLine("You have already selected an order for delivery");
                    return;
                };
                
                navigator.NavigateTo(_delivererScreenFactory.CreateAvailableDeliveriesScreen(navigator, deliverer));
            }),
            new ActionMenuItem("Arrived at restaurant to pick up order", () =>
            {
                _delivererScreenFactory.ShowArrivedAtRestaurantScreen(navigator, deliverer);
            }),
            new ActionMenuItem("Mark this delivery as complete", () =>
            {
                _delivererScreenFactory.CreateCompleteDeliveryScreen(navigator, deliverer);
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout(deliverer.Id);
                Console.WriteLine("You are now logged out.");
                
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                
                navigator.NavigateTo(loginMenu);
                navigator.Clear();
            })
        });
        
        navigator.SetHomeMenu("deliverer", menu);
        return menu;
    }
    
    // CLIENT
    public IMenu CreateMenuForClient(MenuNavigator navigator, Client client)
    {
        var menu = new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                _userDisplayService.DisplayClientInformation(client);
            }),
            new ActionMenuItem("Add item to restaurant menu", () =>
            {
                CreateAddItemToMenuScreen(navigator, client);
            }),
            new ActionMenuItem("See current orders", () =>
            {
                // TODO: THIS IS DEFINITELY NOT A CLEARN SOLUTION. THE FUNCTION SHOULD BE DISPLAYING BUT IT IS IN FACT RETURN A LIST IMENUITEMS
                _restaurantMenuFactory.ShowCurrentOrderScreen(navigator, client);
            }),
            new ActionMenuItem("Start cooking order", () =>
            {
                // TODO: These function need a complete refactor! Very messy and don't like the sumation function...
                navigator.NavigateTo(_restaurantMenuFactory.CreateSelectOrderToCookScreen(navigator, client));
            }),
            new ActionMenuItem("Finish cooking order", () =>
            {
                navigator.NavigateTo(_restaurantMenuFactory.CreateOrderFinishedCookingScreen(navigator, client));
            }),
            new ActionMenuItem("Handle deliverers who have arrived", () =>
            {
                navigator.NavigateTo(_restaurantMenuFactory.CreateHandleMultpleDeliverersArrivalScreen(navigator, client));
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout(client.Id);
                Console.WriteLine("You are now logged out.");
                
                var loginMenu = GetLoginMenu(navigator);
                var registrationMenu = GetRegistrationMenu(navigator, loginMenu);
                LinkLoginToRegistration(loginMenu, registrationMenu, navigator);
                
                navigator.NavigateTo(loginMenu);
                navigator.Clear();
            })
        });
        
        navigator.SetHomeMenu("client", menu);
        return menu;
    }

    // TODO: This function is not an valid Menu Screen!
    public void CreateAddItemToMenuScreen(MenuNavigator navigator, Client client)
    {
        Console.WriteLine("This is your restaurant's current menu:");
        
        // Get the restaurant menu list from client and then show on screen
        var menu = client.GetMenu();
        foreach (var item in menu)
        {
            Console.WriteLine(item);
        }
        // TODO: THIS WHOLE PROCESS MAY NEED TO BE REFACTORED TO FOLLOW SOLID PRINCIPLES
        var message = _restaurantManager.AddMenuItem(client);
        if (!string.IsNullOrWhiteSpace(message)) Console.WriteLine(message);
    }
}