using AribaEats.Interfaces;
using AribaEats.Models.ConsoleFiles;

namespace AribaEats.Models;

public class ClientRegistrar
{
    private readonly UserManager _userManager;
    private readonly ClientInputCollector _inputCollector;
        
    public ClientRegistrar(UserManager userManager)
    {
        _userManager = userManager;
        var validationService = new UserValidationService(userManager);
        _inputCollector = new ClientInputCollector(validationService);
    }
        
    public Client CollectUserInfo()
    {
        return _inputCollector.CollectClientInfo();
    }
        
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