using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Helper;

/// <summary>
/// Handles the registration process for customers.
/// Integrates input collection, validation, and registration using the user management system.
/// Implements the <see cref="IUserRegistrar{Customer}"/> interface specifically for customer registration.
/// </summary>
public class CustomerRegistrar : IUserRegistrar<Customer>
{
    /// <summary>
    /// Manages the collection and persistence of customer user accounts.
    /// </summary>
    private readonly UserManager _userManager;

    /// <summary>
    /// Facilitates the collection and validation of customer-specific input data.
    /// </summary>
    private readonly CustomerInputCollector _inputCollector;

    /// <summary>
    /// Initialises a new instance of the <see cref="CustomerRegistrar"/> class.
    /// Sets up dependencies for user management and customer-specific input handling.
    /// </summary>
    /// <param name="userManager">The <see cref="UserManager"/> instance responsible for managing user accounts.</param>
    public CustomerRegistrar(UserManager userManager)
    {
        _userManager = userManager;
        var validationService = new UserValidationService(userManager);
        _inputCollector = new CustomerInputCollector(validationService);
    }

    /// <summary>
    /// Collects and validates user information required for creating a new customer account.
    /// </summary>
    /// <returns>
    /// A <see cref="Customer"/> object containing customer-specific information collected and validated during the process.
    /// </returns>
    public Customer CollectUserInfo()
    {
        return _inputCollector.CollectCustomerInfo();
    }

    /// <summary>
    /// Registers a new customer user and navigates to the specified menu upon successful registration.
    /// </summary>
    /// <param name="user">The customer user to be registered.</param>
    /// <param name="navigator">
    /// An instance of <see cref="MenuNavigator"/> for managing menu navigation after registration.
    /// </param>
    /// <param name="redirectTo">
    /// The menu where the application navigates after successfully registering the customer.
    /// </param>
    /// <exception cref="Exception">Thrown when user registration fails.</exception>
    public void Register(IUser user, MenuNavigator navigator, IMenu redirectTo)
    {
        bool success = _userManager.AddCustomer((Customer)user);

        if (success)
        {
            Console.WriteLine($"You have been successfully registered as a customer, {user.Name}!");
            navigator.NavigateTo(redirectTo);
        }
        else
        {
            throw new Exception("Could not register user.");
        }
    }
}