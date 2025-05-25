using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Helper;

/// <summary>
/// Handles the registration process for clients.
/// Integrates input collection, validation, and registration through the user management system.
/// Designed specifically to manage client-specific registration workflows.
/// </summary>
public class ClientRegistrar
{
    /// <summary>
    /// Manages and stores client accounts in the system.
    /// </summary>
    private readonly UserManager _userManager;

    /// <summary>
    /// Collects and validates input data specifically for registering a client.
    /// </summary>
    private readonly ClientInputCollector _inputCollector;
        
    /// <summary>
    /// Initialises a new instance of the <see cref="ClientRegistrar"/> class.
    /// Sets up dependencies to facilitate client registration.
    /// </summary>
    /// <param name="userManager">
    /// An instance of <see cref="UserManager"/> for managing user accounts.
    /// </param>
    public ClientRegistrar(UserManager userManager)
    {
        _userManager = userManager;
        var validationService = new UserValidationService(userManager);
        _inputCollector = new ClientInputCollector(validationService);
    }
        
    /// <summary>
    /// Collects all input data required to register a new client.
    /// Uses the client-specific input collector to validate and populate the client information.
    /// </summary>
    /// <returns>
    /// A <see cref="Client"/> object containing the collected and validated data.
    /// </returns>
    public Client CollectUserInfo()
    {
        return _inputCollector.CollectClientInfo();
    }
        
    /// <summary>
    /// Registers a new client in the system and navigates to a specified menu upon successful registration.
    /// </summary>
    /// <param name="user">
    /// The <see cref="Client"/> object representing the user to register.
    /// </param>
    /// <param name="navigator">
    /// A <see cref="MenuNavigator"/> instance for handling navigation after successful registration.
    /// </param>
    /// <param name="redirectTo">
    /// The menu to redirect the user after registration.
    /// </param>
    public void Register(Client user, MenuNavigator navigator, IMenu redirectTo)
    {
        bool success = _userManager.AddClient(user);
            
        if (success)
        {
            Console.WriteLine($"You have been successfully registered as a client, {user.Name}!");
            navigator.NavigateTo(redirectTo);
        }
        else
        {
            Console.WriteLine("Registration failed. Email may already be in use.");
        }
    }
}