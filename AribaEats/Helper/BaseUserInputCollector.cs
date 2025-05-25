using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

/// <summary>
/// Abstract base class that defines a template for collecting user input fields.
/// </summary>
public abstract class BaseUserInputCollector
{
    protected readonly UserValidationService _validationService;
    protected readonly List<IUserInputField> _inputFields;

    protected BaseUserInputCollector(UserValidationService validationService)
    {
        _validationService = validationService;
        _inputFields = new List<IUserInputField>();
        ConfigureInputFields(); // Let the subclass set up specific input fields
    }

    /// <summary>
    /// Template method to be implemented by subclasses to configure specific input fields.
    /// </summary>
    protected abstract void ConfigureInputFields();

    /// <summary>
    /// Displays a prompt and reads user input from the console.
    /// </summary>
    protected string GetUserInput(string info)
    {
        Console.WriteLine($"Please enter your {info}:");
        return Console.ReadLine()!;
    }

    /// <summary>
    /// Loops through each configured input field and collects user data.
    /// </summary>
    public void CollectUserInputInfo(IUser user)
    {
        foreach (var field in _inputFields)
        {
            field.Collect(user, _validationService);
        }
    }
}


/// <summary>
/// Collects and validates user name input.
/// </summary>
public class NameInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public NameInputField(Func<string> getInput) => _getInput = getInput;

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string name = _getInput();
            isValid = validationService.IsValidNameInput(name);
            if (isValid) user.Name = name;
            else Console.WriteLine("Invalid name.");
        }
    }
}


/// <summary>
/// Collects and validates user age input.
/// </summary>
public class AgeInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public AgeInputField(Func<string> getInput) => _getInput = getInput;

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string age = _getInput();
            isValid = validationService.IsValidAge(age);
            if (isValid) user.Age = Convert.ToInt16(age);
            else Console.WriteLine("Invalid age.");
        }
    }
}


/// <summary>
/// Collects and validates email input, checking both format and uniqueness.
/// </summary>
public class EmailInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public EmailInputField(Func<string> getInput) => _getInput = getInput;

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string email = _getInput();
            isValid = validationService.IsValidEmail(email);
            bool isUnique = validationService.IsEmailUnique(email);

            if (isValid && isUnique) user.Email = email;
            else if (isValid && !isUnique)
            {
                Console.WriteLine("This email address is already in use.");
                isValid = false;
            }
            else Console.WriteLine("Invalid email address.");
        }
    }
}


/// <summary>
/// Collects and validates mobile phone input.
/// </summary>
public class MobileInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public MobileInputField(Func<string> getInput) => _getInput = getInput;

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string phone = _getInput();
            isValid = validationService.IsValidMobile(phone);
            if (isValid) user.Mobile = phone;
            else Console.WriteLine("Invalid phone number.");
        }
    }
}

/// <summary>
/// Collects and validates password input with confirmation.
/// </summary>
public class PasswordInputField : IUserInputField
{
    // Delegate used to get user input (not used in this method, but passed in via constructor)
    private readonly Func<string> _getInput;

    // Constructor assigns the provided input function
    public PasswordInputField(Func<string> getInput) => _getInput = getInput;

    // Collect method gathers and validates password input from the user
    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false; // Flag to track whether the password is valid

        // Loop until a valid and confirmed password is entered
        while (!isValid)
        {
            Console.WriteLine("Your password must:\n- be at least 8 characters long\n- contain a number\n- contain a lowercase letter\n- contain an uppercase letter\nPlease enter a password:");

            // Read the first password input
            string input1 = Console.ReadLine()!;

            // Validate the password format using the validation service
            isValid = validationService.IsValidPassword(input1);

            if (isValid)
            {
                // Ask user to confirm the password
                Console.WriteLine("Please confirm your password:");
                string input2 = Console.ReadLine()!;

                // Check if confirmation matches
                if (input1 == input2)
                {
                    // Set the password for the user
                    user.Password = input1;
                }
                else
                {
                    // If passwords don't match, show error and continue loop
                    Console.WriteLine("Passwords do not match.");
                    isValid = false;
                }
            }
            else
            {
                // If password format is invalid, show error and continue loop
                Console.WriteLine("Invalid password.");
            }
        }
    }

}


/// <summary>
/// Collects and validates location (x,y) input from user.
/// </summary>
public class LocationInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public LocationInputField(Func<string> getInput) => _getInput = getInput;

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;

        // Loop until the user provides a valid location
        while (!isValid)
        {
            // Get the location input string from the user (e.g., "2,3")
            string input = _getInput();

            // Split the input by comma into coordinates
            string[] location = input.Split(',');

            // Check if the location format is valid
            isValid = validationService.IsValidLocation(location);

            if (isValid)
            {
                // Convert the coordinates into integers and create a Location object
                Location loc = new Location(Convert.ToInt32(location[0]), Convert.ToInt32(location[1]));

                // Assign the location to the user
                user.Location = loc;

                // If the user is a Client, also update their Restaurant's location
                if (user is Client client)
                    client.Restaurant.Location = loc;
            }
            else
            {
                // If the input is invalid, show an error and repeat
                Console.WriteLine("Invalid location.");
            }
        }
    }
}

/// <summary>
/// Collects and validates licence plate for Deliverer users.
/// </summary>
public class LicencePlateInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public LicencePlateInputField(Func<string> getInput) => _getInput = getInput;

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string input = _getInput();
            isValid = validationService.IsValidLicencePlate(input);
            if (isValid && user is Deliverer deliverer)
                deliverer.LicencePlate = input;
            else Console.WriteLine("Invalid licence plate.");
        }
    }
}

/// <summary>
/// Collects and validates restaurant name input for Client users.
/// </summary>
public class RestaurantNameInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public RestaurantNameInputField(Func<string> getInput) => _getInput = getInput;

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        var restaurant = new Restaurant();
        while (!isValid)
        {
            string input = _getInput();
            isValid = validationService.IsValidRestaurantName(input);
            if (isValid && user is Client client)
            {
                client.Restaurant = restaurant;
                client.Restaurant.Name = input;
                client.Restaurant.ClientId = user.Id;
            }
            else Console.WriteLine("Invalid restaurant name.");
        }
    }
}

/// <summary>
/// Lets the user choose a predefined restaurant style.
/// </summary>
public class RestaurantStyleInputField : IUserInputField
{
    public void Collect(IUser user, UserValidationService validationService)
    {
        // List of available restaurant styles to choose from
        List<string> styleList = new List<string> { "Italian", "French", "Chinese", "Japanese", "American", "Australian" };
    
        int result;       // Will store the user's numeric selection
        bool res = false; // Tracks if a valid input has been received

        // Repeat until a valid option is selected
        do
        {
            // Display available restaurant styles with corresponding numbers
            Console.WriteLine("Please select your restaurant's style:");
            for (int i = 0; i < styleList.Count; i++)
                Console.WriteLine($"{i + 1}: {styleList[i]}");

            Console.WriteLine($"Please enter a choice between 1 and {styleList.Count}:");

            // Read and parse the user input
            res = int.TryParse(Console.ReadLine(), out result);

            // Check if the input is within the valid range
            if (!res || result < 1 || result > styleList.Count)
            {
                Console.WriteLine("Invalid input. Please try again.");
                res = false;
            }

        } while (!res); // Keep asking until valid input is given

        // If the user is a Client, update their restaurant's style
        if (user is Client client)
            client.Restaurant.Style = styleList[result - 1]; // Adjust for zero-based indexing
    }

}
