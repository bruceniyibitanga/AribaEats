using AribaEats;
using AribaEats.Factory;
using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

var userManager = new UserManager();
var restaurantManager = new RestaurantManager();
var userDisplayService = new UserDisplayService();
var delivererManager = new DelivererManager(restaurantManager);
var orderManager = new OrderManager(restaurantManager, userManager, delivererManager);
var orderScreenFactory = new OrderScreenFactory(orderManager, restaurantManager);
var delivererScreenFactory = new DelivererScreenFactory(orderManager, delivererManager);
var restaurantMenuFactory = new RestaurantMenuFactory(restaurantManager, orderScreenFactory, orderManager);


// Create the menu factory (no longer passing registration menu as a parameter)
var menuFactory = new MenuFactory(
    userManager, 
    restaurantManager, 
    orderManager, 
    userDisplayService,
    restaurantMenuFactory,
    delivererManager,
    delivererScreenFactory
);

// Create the navigator with null initial menu
var navigator = new MenuNavigator(null);

// Get the login and registration menus from factory
var loginMenu = menuFactory.GetLoginMenu(navigator);
var registrationMenu = menuFactory.GetRegistrationMenu(navigator, loginMenu);

// Now we can link the login menu to registration menu
menuFactory.LinkLoginToRegistration(loginMenu, registrationMenu, navigator);

// Set the initial menu to login
navigator.SetCurrentMenu(loginMenu);

Console.WriteLine("Welcome to Arriba Eats!");

navigator.Start();