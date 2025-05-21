using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;
using AribaEats.UI;

namespace AribaEats.Helper;

public class DelivererRegistrar : IUserRegistrar<Deliverer>
{
    private readonly UserManager _userManager;
    private readonly DelivererInputCollector _inputCollector;

    public DelivererRegistrar(UserManager userManager)
    {
        _userManager = userManager;
        var validationService = new UserValidationService(userManager);
        _inputCollector = new DelivererInputCollector(validationService);
    }

    public Deliverer CollectUserInfo()
    { 
        return _inputCollector.CollectDelivererInfo();
    }
    
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