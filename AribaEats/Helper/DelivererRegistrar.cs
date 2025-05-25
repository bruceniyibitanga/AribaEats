using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Helper;

/// <summary>
/// Handles the registration process for deliverers.
/// Integrates input collection, validation, and registration via a user management system.
/// Implements the <see cref="IUserRegistrar{Deliverer}"/> interface for deliverer-specific registration.
/// </summary>
public class DelivererRegistrar : IUserRegistrar<Deliverer>
{
    /// <summary>
    /// Manages the collection and persistence of deliverer user accounts.
    /// </summary>
    private readonly UserManager _userManager;

    /// <summary>
    /// Facilitates collection and validation of deliverer-specific input data.
    /// </summary>
    private readonly DelivererInputCollector _inputCollector;

    /// <summary>
    /// Initialises a new instance of the <see cref="DelivererRegistrar"/> class.
    /// Sets up dependencies for user management and deliverer-specific input handling.
    /// </summary>
    /// <param name="userManager">The <see cref="UserManager"/> instance responsible for managing user accounts.</param>
    public DelivererRegistrar(UserManager userManager)
    {
        _userManager = userManager;
        var validationService = new UserValidationService(userManager);
        _inputCollector = new DelivererInputCollector(validationService);
    }

    /// <summary>
    /// Collects and validates user information required for creating a new deliverer account.
    /// </summary>
    /// <returns>
    /// A <see cref="Deliverer"/> object containing all required user information collected during the process.
    /// </returns>
    public Deliverer CollectUserInfo()
    { 
        return _inputCollector.CollectDelivererInfo();
    }

    /// <summary>
    /// Registers a new deliverer user and navigates to the specified menu upon successful registration.
    /// </summary>
    /// <param name="user">The deliverer user to be registered.</param>
    /// <param name="navigator">
    /// An instance of <see cref="MenuNavigator"/> for handling menu navigation after registration.
    /// </param>
    /// <param name="redirectTo">
    /// The menu to redirect to after successful registration.
    /// </param>
    /// <exception cref="Exception">Thrown when user registration fails.</exception>
    public void Register(IUser user, MenuNavigator navigator, IMenu redirectTo)
    {
        bool success = _userManager.AddDeliverer((Deliverer)user);

        if (success)
        {
            Console.WriteLine($"You have been successfully registered as a deliverer, {user.Name}!");
            navigator.NavigateTo(redirectTo);
        }
        else
        {
            throw new Exception("Failed to register user");
        }
    }
}