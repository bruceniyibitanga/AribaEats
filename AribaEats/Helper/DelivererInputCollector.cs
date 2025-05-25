using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

/// <summary>
/// Responsible for collecting and validating input required to create a new deliverer account.
/// Inherits from the <see cref="BaseUserInputCollector"/> to handle user data collection.
/// Adds deliverer-specific fields such as the license plate.
/// </summary>
public class DelivererInputCollector : BaseUserInputCollector
{
    /// <summary>
    /// A service used for validating user input during the collection process.
    /// </summary>
    private readonly UserValidationService _validationService;

    /// <summary>
    /// Initialises a new instance of the <see cref="DelivererInputCollector"/> class with the required validation service.
    /// </summary>
    /// <param name="validationService">
    /// An instance of <see cref="UserValidationService"/> to ensure all input fields are valid.
    /// </param>
    public DelivererInputCollector(UserValidationService validationService) : base(validationService)
    {
        _validationService = validationService;
    }

    /// <summary>
    /// Configures the input fields specific to the deliverer user type.
    /// Adds fields such as name, age, email, mobile number, password, and license plate using specific input field classes.
    /// </summary>
    protected override void ConfigureInputFields()
    {
        _inputFields.Add(new NameInputField(() => GetUserInput("name")));
        _inputFields.Add(new AgeInputField(() => GetUserInput("age (18-100)")));
        _inputFields.Add(new EmailInputField(() => GetUserInput("email address")));
        _inputFields.Add(new MobileInputField(() => GetUserInput("mobile phone number")));
        _inputFields.Add(new PasswordInputField(() => GetUserInput("password")));
        _inputFields.Add(new LicencePlateInputField(() => GetUserInput("licence plate")));
    }

    /// <summary>
    /// Collects all required information to create a new deliverer account.
    /// Uses the configured input fields to gather and validate user input.
    /// </summary>
    /// <returns>
    /// A <see cref="Deliverer"/> object populated with the collected and validated input data.
    /// </returns>
    public Deliverer CollectDelivererInfo()
    {
        var deliverer = new Deliverer();
        CollectUserInputInfo(deliverer);
        return deliverer;
    }
}