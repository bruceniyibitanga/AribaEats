using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

/// <summary>
/// Responsible for collecting and validating input specific to a customer's registration process.
/// Extends the functionality of <see cref="BaseUserInputCollector"/> to handle customer-specific input fields.
/// </summary>
public class CustomerInputCollector : BaseUserInputCollector
{
    /// <summary>
    /// Initialises a new instance of the <see cref="CustomerInputCollector"/> class.
    /// Sets up the validation service and input fields specific to customer registration.
    /// </summary>
    /// <param name="validationService">
    /// An instance of <see cref="UserValidationService"/> to ensure all customer input is valid.
    /// </param>
    public CustomerInputCollector(UserValidationService validationService) : base(validationService)
    {
    }

    /// <summary>
    /// Configures the input fields required for customer registration.
    /// Includes fields such as name, age, email, mobile phone number, password, and location.
    /// </summary>
    protected override void ConfigureInputFields()
    {
        _inputFields.Add(new NameInputField(() => GetUserInput("name")));
        _inputFields.Add(new AgeInputField(() => GetUserInput("age (18-100)")));
        _inputFields.Add(new EmailInputField(() => GetUserInput("email address")));
        _inputFields.Add(new MobileInputField(() => GetUserInput("mobile phone number")));
        _inputFields.Add(new PasswordInputField(() => GetUserInput("password")));
        _inputFields.Add(new LocationInputField(() => GetUserInput("location (in the form of X,Y)")));
    }

    /// <summary>
    /// Collects all required information to register a new customer.
    /// Gathers input for all configured fields and validates the data.
    /// </summary>
    /// <returns>
    /// A <see cref="Customer"/> object populated with the collected and validated input data.
    /// </returns>
    public Customer CollectCustomerInfo()
    {
        var customer = new Customer();
        CollectUserInputInfo(customer);
        return customer;
    }
}