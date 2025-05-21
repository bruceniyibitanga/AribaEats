namespace AribaEats.Models;

public class CustomerInputCollector :BaseUserInputCollector
{
    public CustomerInputCollector(UserValidationService validationService) : base(validationService)
    {
    }

    protected override void ConfigureInputFields()
    {
        _inputFields.Add(new NameInputField(() => GetUserInput("name")));
        _inputFields.Add(new AgeInputField(() => GetUserInput("age (18-100)")));
        _inputFields.Add(new EmailInputField(() => GetUserInput("email address")));
        _inputFields.Add(new MobileInputField(() => GetUserInput("mobile phone number")));
        _inputFields.Add(new PasswordInputField(() => GetUserInput("password")));
        _inputFields.Add(new LocationInputField(() => GetUserInput("location (in the form of X,Y)")));
    }

    public Customer CollectCustomerInfo()
    {
        var customer = new Customer();
        CollectUserInputInfo(customer);
        return customer;
    }
}