using AribaEats.Factory;
using AribaEats.Models;
using AribaEats.Services;

// Initialize core service managers that handle different aspects of the application
var userManager = new UserManager();                
var restaurantManager = new RestaurantManager();    
var userDisplayService = new UserDisplayService();  

// Create delivery management system with restaurant integration
var delivererManager = new DelivererManager();  
                                                                

// Initialize order processing system with all required dependencies
var orderManager = new OrderManager(
    restaurantManager,   
    userManager,         
    delivererManager);   

// Set up UI factories for different sections of the application
var orderScreenFactory = new OrderScreenFactory(
    orderManager,        
    restaurantManager);  

var delivererScreenFactory = new DelivererScreenFactory(
    orderManager,        
    delivererManager);   

var restaurantMenuFactory = new RestaurantMenuFactory(
    restaurantManager,   
    orderScreenFactory,  
    orderManager);       

// Create the main menu factory that coordinates all application screens
// This factory is responsible for creating all main navigation menus and their interconnections
var menuFactory = new MenuFactory(
    userManager,            
    restaurantManager,      
    orderManager,           
    userDisplayService,     
    restaurantMenuFactory,  
    delivererManager,       
    delivererScreenFactory  
);

// Initialize the navigation system that manages menu transitions
// Starts with null as the initial menu will be set after creating required menus
var navigator = new MenuNavigator(null);

// Create and configure login process.
var loginMenu = menuFactory.GetLoginMenu(navigator);
var registrationMenu = menuFactory.GetRegistrationMenu(navigator, loginMenu);  

// Set up navigation between authentication screens
// This allows users to switch between login and registration as needed
menuFactory.LinkLoginToRegistration(loginMenu, registrationMenu, navigator);

// Configure the application to start at the login screen
navigator.SetCurrentMenu(loginMenu);

// Display welcome message and start the application's main loop
Console.WriteLine("Welcome to Arriba Eats!");
navigator.Start();  // Begins the interactive menu system that handles user input and navigation