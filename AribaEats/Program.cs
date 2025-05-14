using AribaEats;
using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Models.Console;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
        RegisterUser(new Customer(), "customer", navigator, userManager, loginMenu)));

    // Update Deliverer menu item
    consoleMenu.EditMenuItem(1, new ActionMenuItem("Deliverer", () =>
        RegisterUser(new Deliverer(), "deliverer", navigator, userManager, loginMenu)));

    // Update Client menu item
    consoleMenu.EditMenuItem(2, new ActionMenuItem("Client", () =>
        RegisterUser(new Client(), "client", navigator, userManager, loginMenu)));
}


void RegisterUser(IUser user, string userType, MenuNavigator navigator, UserManager userManager, IMenu redirectTo)
{
    CreateNewUser(user, userManager);
    userManager.AddCustomer((Customer)user);
    Console.WriteLine($"You have been successfully registered as a {userType}, {user.Name}!");
    navigator.NavigateTo(redirectTo);
}

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

            var user = userManager.LoginAsCustomer(email, password);

            if (user.ProfileExists & user.PasswordCorrect)
            {
                Console.WriteLine($"Welcome back, {user.User!.Name}!");
                navigator.NavigateTo(CreateCustomerMenu(navigator, user.User));
            }
            else
                Console.WriteLine("Invalid email or password.");
        }),
        new NavigateMenuItem("Register as a new user", registrationMenu, navigator),
        new ActionMenuItem("Exit", () => {Console.WriteLine("Thank you for using Arriba Eats!"); Environment.Exit(0); })
    });
}

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
            navigator.NavigateTo(CreateRestrauntSortMenu(navigator, customer, restaurantManager));
        }),
        new ActionMenuItem("See the status of your orders", () =>
        {
            if (customer.GetOrderCount() == 0) Console.WriteLine("You have not placed any orders.");
            // TODO: REALLY THINK ABOUT THE ORDER FUNCTIONALITY AND WHETHER THERE SHOULD BE AN ORDER MANAGER?!
        }),
        new ActionMenuItem("Rate a restaurant you've ordered from", () =>
        {
            // if(customer.GetOrderCount() == 0) Console.WriteLine("You have not placed any orders.");
            navigator.NavigateTo(CreateRestaurantOrderedFromMenu(navigator, customer, orderManager));
        }),
        new ActionMenuItem("Log out", () =>
        {
            userManager.Logout();
            Console.WriteLine("You are now logged out.");
            
            // If a user has logged out. We want to clear the navigation history. And then redirect them to the login page.
            // This will help solve issue so that a user can't redirect themselves back as a logged-in user?
            navigator.NavigateTo(CreateLoginMenu(navigator, userManager, registrationMenu));
            navigator.Clear();
            
        })
    });
}

// RESTAURANT MENUS
IMenu CreateRestrauntSortMenu(MenuNavigator navigator, Customer customer, RestaurantManager restaurantManager)
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


IMenu CreateRestrauntListMenu(MenuNavigator navigator, Customer user, RestaurantManager restaurantManager, string sortOrder)
{
    // If the user hasn't made an order before, we should display message accordingly.
    if (user.GetOrderCount() == 0) Console.WriteLine("You have not placed any orders."); navigator.NavigateTo(CreateCustomerMenu(navigator, user));
    
    // TODO: Not really what we want. We want to return a new Table.
    return new ConsoleMenu("You can order from the following restaurants:", new IMenuItem[]
    {

    });
}

IMenu CreateRestaurantOrderedFromMenu(MenuNavigator navigator, Customer customer, OrderManager orderManager)
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



// TODO: This may need to be refactored into the UserManager
void CreateNewUser(IUser user, UserManager userManager)
{
    bool isValid = false;

    while (!isValid)
    {
        string name = GetUserInput("name");
        isValid = IsValidNameInput(name);
        if (isValid)
        {
            user.Name = name;
        }
        else Console.WriteLine("Invalid name.");
    }

    isValid = false;

    while (!isValid)
    {
        string age = GetUserInput("age (18-100)");
        isValid = IsValidAge(age);
        if (isValid)
        {
            user.Age = Convert.ToInt16(age);
        }
        else Console.WriteLine("Invalid age.");
    }

    isValid = false;

    while (!isValid)
    {
        string email = GetUserInput("email address");
        isValid = IsValidEmail(email);
        bool isUnique = userManager.IsEmailUnique(email);
        
        if (isValid && isUnique)
        {
            user.Email = email;
        }

        if (isValid & !isUnique)
        {
            Console.WriteLine("This email address is already in use.");
            isValid = false;
        }
        else if(!isValid & isUnique) Console.WriteLine("Invalid email address.");
    }

    isValid = false;

    while (!isValid)
    {
        string phone = GetUserInput("mobile phone number");
        isValid = IsValidMobile(phone);
        if (isValid)
        {
            user.Mobile = phone;
        }
        else Console.WriteLine("Invalid phone number.");
    }

    isValid = false;

    while (!isValid)
    {
        Console.WriteLine("Your password must:\r\n- be at least 8 characters long\r\n- contain a number\r\n- contain a lowercase letter\r\n- contain an uppercase letter\r\nPlease enter a password:");
        string input1 = Console.ReadLine();
        isValid = IsValidPassword(input1);

        if (isValid)
        {
            Console.WriteLine("Please confirm your password:");
            string input2 = Console.ReadLine();

            if (input1 == input2)
            {
                user.Password = input1;
            }
            else
            {
                Console.WriteLine("Passwords do not match.");
                isValid = false;
            }
        }
        else Console.WriteLine("Invalid password.");
    }

    isValid = false;

    while (!isValid)
    {
        string input = GetUserInput("location (in the form of X,Y)");
        string[] location = input.Split(',');
        isValid = IsValidLocation(location);

        if (isValid)
        {
            Location loc = new Location(Convert.ToInt32(location[0]), Convert.ToInt32(location[1]));
            user.Location = loc;
        }
        else
        {
            Console.WriteLine("Invalid location.");
        }
    }
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

string GetUserInput(string info)
{
    Console.WriteLine($"Please enter your {info}:");
    return Console.ReadLine();
}

static bool IsValidNameInput(string name)
{
    if (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(name, @"^[a-zA-Z\s'-]+$") || !Regex.IsMatch(name, @"[a-zA-Z]")) return false;

    return true;
}

static bool IsValidAge(string input)
{
    if (int.TryParse(input, out int age))
    {
        return age >= 18 && age <= 100;
    }
    return false;
}

static bool IsValidMobile(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return false;

    if (input.Length != 10)
        return false;

    if (input[0] != '0')
        return false;

    // Check if all characters are digits
    foreach (char c in input)
    {
        if (!char.IsDigit(c))
            return false;
    }

    return true;
}

static bool IsValidEmail(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return false;

    int atIndex = input.IndexOf('@');
    int lastAtIndex = input.LastIndexOf('@');

    // Check exactly one @
    if (atIndex == -1 || atIndex != lastAtIndex)
        return false;

    // Check at least one character before and after @
    if (atIndex == 0 || atIndex == input.Length - 1)
        return false;

    return true;
}

static bool IsValidPassword(string input)
{
    if (string.IsNullOrEmpty(input)) return false;

    // Regex to check at least one lowercase, one uppercase, one digit, and minimum length of 8
    return Regex.IsMatch(input, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
}

static bool IsValidLocation(string[] location)
{
    if (location.Length != 2) return false;

    foreach (string value in location)
    {
        bool canParse = int.TryParse(value, out int result);

        if (!canParse) return false;
    }

    return true;
}

static bool IsValidLicencePlate(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return false;

    // Regex to check length and allowed characters (uppercase letters, numbers, and spaces)
    return Regex.IsMatch(input, @"^[A-Z0-9 ]{1,8}$") && !string.IsNullOrWhiteSpace(input);
}

navigator.Start();