using AribaEats;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Models.ConsoleFiles;

var userManager = new UserManager();
var restaurantManager = new RestaurantManager();
var orderManager = new OrderManager(restaurantManager);
var navigator = new MenuNavigator(null);

// Declare menu variables first
IMenu loginMenu;
IMenu registrationMenu;

registrationMenu = CreateRegistrationMenuPlaceholder(navigator);

loginMenu = CreateLoginMenu(navigator, userManager, registrationMenu);

// Update the registration menu with proper actions
UpdateRegistrationMenu(registrationMenu, navigator, userManager, loginMenu);

// Update current menu screen
navigator.SetCurrentMenu(loginMenu);

Console.WriteLine("Welcome to Arriba Eats!");

// Method to create a placeholder registration menu
IMenu CreateRegistrationMenuPlaceholder(MenuNavigator navigator)
{
    return new ConsoleMenu(
        "Which type of user would you like to register as?",
    new IMenuItem[]
    {
        new ActionMenuItem("Customer", () => { /* Placeholder */ }),
        new ActionMenuItem("Deliverer", () => { /* Placeholder */ }),
        new ActionMenuItem("Client", () => { /* Placeholder */ }),
        new BackMenuItem("Return to the previous menu", navigator)
    });
}

// Method to update the registration menu with proper actions
void UpdateRegistrationMenu(IMenu registrationMenu, MenuNavigator navigator, UserManager userManager, IMenu loginMenu)
{
    var consoleMenu = (ConsoleMenu)registrationMenu;

    // Update Customer menu item
    consoleMenu.EditMenuItem(0, new ActionMenuItem("Customer", () =>
    {
        var registrar = new CustomerRegistrar(userManager);
        var customer = registrar.CollectUserInfo();
        registrar.Register(customer, navigator, loginMenu);
    }));

    // Update Deliverer menu item
    consoleMenu.EditMenuItem(1, new ActionMenuItem("Deliverer", () =>
    {
        var registrar = new DelivererRegistrar(userManager);
        var deliverer = registrar.CollectUserInfo();
        registrar.Register(deliverer, navigator, loginMenu);
    }));

    // Update Client menu item
    consoleMenu.EditMenuItem(2, new ActionMenuItem("Client", () =>
    {
        var registrar = new ClientRegistrar(userManager);
        var client = registrar.CollectUserInfo();
        registrar.Register(client, navigator, loginMenu);
    }));
}

navigator.Start();