using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

/// <summary>
/// Collects and validates input specific to a client during registration.
/// Inherits from <see cref="BaseUserInputCollector"/> for general user data collection functionality,
/// and adds client-specific input fields, such as restaurant information.
/// </summary>
public class ClientInputCollector : BaseUserInputCollector
{
    /// <summary>
    /// Initialises a new instance of the <see cref="ClientInputCollector"/> class.
    /// Configures the input fields required for collecting client registration data.
    /// </summary>
    /// <param name="validationService">
    /// An instance of <see cref="UserValidationService"/> to validate the inputs collected during registration.
    /// </param>
    public ClientInputCollector(UserValidationService validationService) : base(validationService)
    {
    }

    /// <summary>
    /// Configures the input fields specific to client registration.
    /// Includes basic user details like name, age, and email, as well as fields for restaurant name, style, and location.
    /// </summary>
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

    /// <summary>
    /// Collects all required input for client registration.
    /// Uses the configured input fields to gather and validate data, returning a populated <see cref="Client"/> object.
    /// </summary>
    /// <returns>
    /// A <see cref="Client"/> object with all validated input data.
    /// </returns>
    public Client CollectClientInfo()
    {
        var client = new Client();
        CollectUserInputInfo(client);
        return client;
    }
}