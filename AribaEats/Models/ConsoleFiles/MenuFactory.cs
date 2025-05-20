using AribaEats.Interfaces;

namespace AribaEats.Models.ConsoleFiles;

public class MenuFactory : IMenuFactory
{
    private readonly UserManager _userManager;
    private readonly RestaurantManager _restaurantManager;
    private readonly OrderManager _orderManager;
    private readonly IMenu _loginMenu;
    public MenuFactory(UserManager userManager, RestaurantManager restaurantManager, OrderManager orderManager, IMenu loginMenu)
    {
        _userManager = userManager;
        _restaurantManager = restaurantManager;
        _orderManager = orderManager;
        _loginMenu = loginMenu;
    }
    
    public IMenu CreateMenuFor(IUser user, MenuNavigator navigator)
    {
        return user switch
        {
            Customer customer => CreateMenuForCustomer(navigator, customer),
            Deliverer deliverer => CreateMenuForDeliverer(navigator, deliverer),
            Client client => CreateMenuForClient(navigator, client)
        };
    }
    
    // LOGIN MENU
    IMenu CreateLoginMenu(MenuNavigator navigator, UserManager userManager, IMenu registrationMenu)
    {
        return new ConsoleMenu(
        // TODO: THIS LINE IS OFTEN REPEATED AND MAYBE SHOULD BE REFACTORED OUT TO APPEAR WITHOUT REPITION
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

                var user = userManager.Login(email, password);

                if (user.ProfileExists & user.PasswordCorrect)
                {
                    Console.WriteLine($"Welcome back, {user.User!.Name}!");
                    navigator.NavigateTo(CreateMenuFor(user.User, navigator));
                }
                else
                    Console.WriteLine("Invalid email or password.");
            }),
            new NavigateMenuItem("Register as a new user", registrationMenu, navigator),
            new ActionMenuItem("Exit", () => {Console.WriteLine("Thank you for using Arriba Eats!"); Environment.Exit(0); })
        });
    }
    
    // CUSTOMER MENUS
    IMenu CreateCustomerMenu(MenuNavigator navigator, Customer customer)
    {
        return new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                DisplayUserInformation(customer);
                // TODO: HIGH COUPLING. WE SHOULDN'T BE DIRECTELY UPDATING THE X, Y LOCATION. HAVE A METHOD THAT SETS THESE.
                // REFER TO LECTURE SLIDES FOR WK 10 IF NOT SURE!!!!
                
                Console.WriteLine($"Location: {customer.Location.X},{customer.Location.Y}");
                Console.WriteLine(customer.GetOrderSummary());
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
                // if(customer.GetOrderCount() == 0) Console.WriteLine("You have not placed any orders.");
                navigator.NavigateTo(CreateRestaurantOrderedFromMenu(navigator, customer, _orderManager));
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout();
                Console.WriteLine("You are now logged out.");
                
                // If a user has logged out. We want to clear the navigation history. And then redirect them to the login page.
                // This will help solve issue so that a user can't redirect themselves back as a logged-in user?
                navigator.NavigateTo(CreateLoginMenu(navigator, _userManager, _loginMenu));
                navigator.Clear();
            })
        });
    }

    private IMenu CreateMenuForCustomer(MenuNavigator navigator, Customer customer)
    {
        return new ConsoleMenu("Please make a choice from the menu below:", new IMenuItem[]
        {
            new ActionMenuItem("Display your user information", () =>
            {
                DisplayUserInformation(customer);
                // TODO: HIGH COUPLING. WE SHOULDN'T BE DIRECTELY UPDATING THE X, Y LOCATION. HAVE A METHOD THAT SETS THESE.
                // REFER TO LECTURE SLIDES FOR WK 10 IF NOT SURE!!!!
                
                Console.WriteLine($"Location: {customer.Location.X},{customer.Location.Y}");
                Console.WriteLine(customer.GetOrderSummary());
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
                // if(customer.GetOrderCount() == 0) Console.WriteLine("You have not placed any orders.");
                navigator.NavigateTo(CreateRestaurantOrderedFromMenu(navigator, customer, _orderManager));
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout();
                Console.WriteLine("You are now logged out.");
                
                // If a user has logged out. We want to clear the navigation history. And then redirect them to the login page.
                // This will help solve issue so that a user can't redirect themselves back as a logged-in user?
                navigator.NavigateTo(CreateLoginMenu(navigator, _userManager, _loginMenu));
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
            new ActionMenuItem("Return to the previous menu", () => navigator.NavigateTo(CreateCustomerMenu(navigator, customer)))
        });
    }

    private IMenu CreateRestrauntListMenu(MenuNavigator navigator, Customer user, RestaurantManager restaurantManager, string sortOrder)
    {
        // If the user hasn't made an order before, we should display message accordingly.
        if (user.GetOrderCount() == 0) Console.WriteLine("You have not placed any orders."); navigator.NavigateTo(CreateCustomerMenu(navigator, user));
        
        // TODO: Not really what we want. We want to return a new Table.
        return new ConsoleMenu("You can order from the following restaurants:", new IMenuItem[]
        {

        });
    }

    private IMenu CreateRestaurantOrderedFromMenu(MenuNavigator navigator, Customer customer, OrderManager orderManager)
    {
        List<string> orderedOrderStatuses = orderManager.GetOrderStatuses(customer.Id);

        var menuItems = new List<IMenuItem>();

        foreach (var orderStatus in orderedOrderStatuses)
        {
            menuItems.Add(new ActionMenuItem(orderStatus, () =>
            {
            }));
        }

        menuItems.Add(new ActionMenuItem("Return to the previous menu", () => navigator.NavigateBack()));
        
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
                DisplayUserInformation(deliverer);
                // TODO: HIGH COUPLING. WE SHOULDN'T BE DIRECTELY UPDATING THE X, Y LOCATION. HAVE A METHOD THAT SETS THESE.
                // REFER TO LECTURE SLIDES FOR WK 10 IF NOT SURE!!!!
                
                Console.WriteLine($"License plate: {deliverer.LicensePlate}");
                Console.WriteLine(deliverer.GetCurrentDeliveryStatus());
            }),
            new ActionMenuItem("List orders available to deliver", () =>
            {
            }),
            new ActionMenuItem("Arrived at restaurant to pick up order", () =>
            {
            }),
            new ActionMenuItem("Mark this delivery as complete", () =>
            {
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout();
                Console.WriteLine("You are now logged out.");
                
                // If a user has logged out. We want to clear the navigation history. And then redirect them to the login page.
                // This will help solve issue so that a user can't redirect themselves back as a logged-in user?
                navigator.NavigateTo(CreateLoginMenu(navigator, _userManager, _loginMenu));
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
                DisplayUserInformation(client);
                // TODO: HIGH COUPLING. WE SHOULDN'T BE DIRECTELY UPDATING THE X, Y LOCATION. HAVE A METHOD THAT SETS THESE.
                // REFER TO LECTURE SLIDES FOR WK 10 IF NOT SURE!!!!
                
                Console.WriteLine($"Restaurant name: {client.RestaurantName}");
                Console.WriteLine($"Restaurant style: {client.RestaurantStyle}");
                Console.WriteLine($"Restaurant location: {client.GetRestaurantLocation()}");
            }),
            new ActionMenuItem("List orders available to deliver", () =>
            {
            }),
            new ActionMenuItem("Arrived at restaurant to pick up order", () =>
            {
            }),
            new ActionMenuItem("Mark this delivery as complete", () =>
            {
            }),
            new ActionMenuItem("Log out", () =>
            {
                _userManager.Logout();
                Console.WriteLine("You are now logged out.");
                
                // If a user has logged out. We want to clear the navigation history. And then redirect them to the login page.
                // This will help solve issue so that a user can't redirect themselves back as a logged-in user?
                navigator.NavigateTo(CreateLoginMenu(navigator, _userManager, _loginMenu));
                navigator.Clear();
                
            })
        });
    }
    
    // TODO: This function will need to be refactored into an abstract class like "UserServices" or ask ChatGPT
    // Do this for all the functions in here and have them in classes that have same functionality.
    // Function to display the user details
    void DisplayUserInformation(IUser user)
    {
        Console.WriteLine("Your user details are as follows:");
        Console.WriteLine($"Name: {user.Name}");
        Console.WriteLine($"Age: {user.Age}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Mobile: {user.Mobile}");
    }
}