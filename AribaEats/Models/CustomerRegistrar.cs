using AribaEats.Interfaces;
using AribaEats.Models.ConsoleFiles;

namespace AribaEats.Models;

public class CustomerRegistrar: IUserRegistrar<Customer>
{
    private readonly UserManager _userManager;
    private readonly CustomerInputCollector _inputCollector;

    public CustomerRegistrar(UserManager userManager)
    {
        _userManager = userManager;
        var validationService = new UserValidationService(userManager);
        _inputCollector = new CustomerInputCollector(validationService);
    }
    
    public Customer CollectUserInfo()
    {
        return _inputCollector.CollectCustomerInfo();
    }

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