using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

public class DelivererInputCollector : BaseUserInputCollector
{
    private readonly UserValidationService _validationService;
    public DelivererInputCollector(UserValidationService validationService) :base(validationService)
    {
        _validationService = validationService;
    }

    protected override void ConfigureInputFields()
    {
        _inputFields.Add(new NameInputField(() => GetUserInput("name")));
        _inputFields.Add(new AgeInputField(() => GetUserInput("age (18-100)")));
        _inputFields.Add(new EmailInputField(() => GetUserInput("email address")));
        _inputFields.Add(new MobileInputField(() => GetUserInput("mobile phone number")));
        _inputFields.Add(new PasswordInputField(() => GetUserInput("password")));
        _inputFields.Add(new LicencePlateInputField(() => GetUserInput("licence plate")));
    }

    public Deliverer CollectDelivererInfo()
    {
        var deliverer = new Deliverer();
        CollectUserInputInfo(deliverer);
        return deliverer;
    }
}