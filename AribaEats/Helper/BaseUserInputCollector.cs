using AribaEats.Interfaces;
using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

public abstract class BaseUserInputCollector
{
    protected readonly UserValidationService _validationService;
    protected readonly List<IUserInputField> _inputFields;

    protected BaseUserInputCollector(UserValidationService validationService)
    {
        _validationService = validationService;
        _inputFields = new List<IUserInputField>();
        ConfigureInputFields();
    }

    // Template method for subclasses to define their input fields
    protected abstract void ConfigureInputFields();

    // Generic method to get user input
    protected string GetUserInput(string info)
    {
        Console.WriteLine($"Please enter your {info}:");
        return Console.ReadLine()!;
    }

    // Collect all configured input fields
    public void CollectUserInputInfo(IUser user)
    {
        foreach (var field in _inputFields)
        {
            field.Collect(user, _validationService);
        }
    }
}

public class NameInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public NameInputField(Func<string> getInput)
    {
        _getInput = getInput;
    }

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string name = _getInput();
            isValid = validationService.IsValidNameInput(name);
            if (isValid)
            {
                user.Name = name;
            }
            else Console.WriteLine("Invalid name.");
        }
    }
}

public class AgeInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public AgeInputField(Func<string> getInput)
    {
        _getInput = getInput;
    }

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string age = _getInput();
            isValid = validationService.IsValidAge(age);
            if (isValid)
            {
                user.Age = Convert.ToInt16(age);
            }
            else Console.WriteLine("Invalid age.");
        }
    }
}

public class EmailInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public EmailInputField(Func<string> getInput)
    {
        _getInput = getInput;
    }

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string email = _getInput();
            isValid = validationService.IsValidEmail(email);
            bool isUnique = validationService.IsEmailUnique(email);
            
            if (isValid && isUnique)
            {
                user.Email = email;
            }
            else if (isValid && !isUnique)
            {
                Console.WriteLine("This email address is already in use.");
                isValid = false;
            }
            else if (!isValid) Console.WriteLine("Invalid email address.");
        }
    }
}

public class MobileInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public MobileInputField(Func<string> getInput)
    {
        _getInput = getInput;
    }

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string phone = _getInput();
            isValid = validationService.IsValidMobile(phone);
            if (isValid)
            {
                user.Mobile = phone;
            }
            else Console.WriteLine("Invalid phone number.");
        }
    }
}

public class PasswordInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public PasswordInputField(Func<string> getInput)
    {
        _getInput = getInput;
    }

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            Console.WriteLine("Your password must:\r\n- be at least 8 characters long\r\n- contain a number\r\n- contain a lowercase letter\r\n- contain an uppercase letter\r\nPlease enter a password:");
            string input1 = Console.ReadLine()!;
            isValid = validationService.IsValidPassword(input1);
            if (isValid)
            {
                Console.WriteLine("Please confirm your password:");
                string input2 = Console.ReadLine()!;
                if (input1 == input2)
                {
                    user.Password = input1;
                }
                else
                {
                    Console.WriteLine("Passwords do not match.");
                    isValid = false;
                }
            }
            else Console.WriteLine("Invalid password.");
        }
    }
}

public class LocationInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public LocationInputField(Func<string> getInput)
    {
        _getInput = getInput;
    }

    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string input = _getInput();
            string[] location = input.Split(',');
            isValid = validationService.IsValidLocation(location);
            if (isValid)
            {
                Location loc = new Location(Convert.ToInt32(location[0]), Convert.ToInt32(location[1]));
                user.Location = loc;
            }
            else
            {
                Console.WriteLine("Invalid location.");
            }
        }
    }
}

public class LicencePlateInputField : IUserInputField
{
    private readonly Func<string> _getInput;

    public LicencePlateInputField(Func<string> getInput)
    {
        _getInput = getInput;
    }
    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string input = _getInput();
            isValid = validationService.IsValidLicencePlate(input);
            if (isValid && user.GetType() == typeof(Deliverer)) ((Deliverer)user).LicencePlate = input;
            else Console.WriteLine("Invalid licence plate.");
        }
    }
}

public class RestaurantNameInputField :IUserInputField
{
    private readonly Func<string> _getInput;

    public RestaurantNameInputField(Func<string> getInput)
    {
        _getInput = getInput;
    }
    public void Collect(IUser user, UserValidationService validationService)
    {
        bool isValid = false;
        while (!isValid)
        {
            string input = _getInput();
            isValid = validationService.IsValidRestaurantName(input);
            if (isValid && user.GetType() == typeof(Client)) ((Client)user).Restaurant.Name = input;
            else Console.WriteLine("Invalid restaurant name.");
        }
    }
}

public class RestaurantStyleInputField : IUserInputField
{
    public void Collect(IUser user, UserValidationService validationService)
    {
        List<string> styleList = new List<string> { "Italian", "French", "Chinese", "Japanese", "American", "Australian" };
        int result;
        bool res = false;
        do
        {
            Console.WriteLine("Please select your restaurant's style:");
            for (int i = 0; i < styleList.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {styleList[i]}");
            }
            Console.WriteLine($"Please enter a choice between 1 and {styleList.Count}:");
            res = int.TryParse(Console.ReadLine(), out result);
            // Validate the input is within the correct range
            if (!res || result < 1 || result > styleList.Count)
            {
                Console.WriteLine("Invalid input. Please try again.");
                res = false;
            }
        } while (!res);
        ((Client)user).Restaurant.Style = styleList[result - 1];
    }
}