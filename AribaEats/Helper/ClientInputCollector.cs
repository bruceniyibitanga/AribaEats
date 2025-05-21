using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

public class ClientInputCollector : BaseUserInputCollector
{
    public ClientInputCollector(UserValidationService validationService) : base(validationService)
    {
    }
    protected override void ConfigureInputFields()
    {
        _inputFields.Add(new NameInputField(() => GetUserInput("name")));
        _inputFields.Add(new AgeInputField(() => GetUserInput("age (18-100)")));
        _inputFields.Add(new EmailInputField(() => GetUserInput("email address")));
        _inputFields.Add(new MobileInputField(() => GetUserInput("mobile phone number")));
        _inputFields.Add(new PasswordInputField(() => GetUserInput("password")));
        _inputFields.Add(new RestaurantNameInputField(() => GetUserInput("restaurant's name")));
        _inputFields.Add(new RestaurantStyleInputField());
        _inputFields.Add(new LocationInputField(() => GetUserInput("location (in the form of X,Y)")));
    }

    public Client CollectClientInfo()
    {
        var client = new Client();
        CollectUserInputInfo(client);
        return client;
    }
}