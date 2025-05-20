using AribaEats.Interfaces;
namespace AribaEats.Models;

public abstract class BaseUserInputCollector
{
    private readonly UserValidationService _validationService;

    protected BaseUserInputCollector(UserValidationService validationService)
    {
        _validationService = validationService;
    }

    protected virtual bool ShouldPromptForLocation() => true;

    protected string GetUserInput(string info)
    {
        Console.WriteLine($"Please enter your {info}:");
        return Console.ReadLine()!;
    }
    
    public void CollectBaseUserInfo(IUser user)
    {
        bool isValid = false;

        while (!isValid)
        {
            string name = GetUserInput("name");
            isValid = _validationService.IsValidNameInput(name);
            if (isValid)
            {
                user.Name = name;
            }
            else Console.WriteLine("Invalid name.");
        }

        isValid = false;

        while (!isValid)
        {
            string age = GetUserInput("age (18-100)");
            isValid = _validationService.IsValidAge(age);
            if (isValid)
            {
                user.Age = Convert.ToInt16(age);
            }
            else Console.WriteLine("Invalid age.");
        }

        isValid = false;

        while (!isValid)
        {
            string email = GetUserInput("email address");
            isValid = _validationService.IsValidEmail(email);
            bool isUnique = _validationService.IsEmailUnique(email);
            
            if (isValid && isUnique)
            {
                user.Email = email;
            }

            if (isValid & !isUnique)
            {
                Console.WriteLine("This email address is already in use.");
                isValid = false;
            }
            else if(!isValid & isUnique) Console.WriteLine("Invalid email address.");
        }

        isValid = false;

        while (!isValid)
        {
            string phone = GetUserInput("mobile phone number");
            isValid = _validationService.IsValidMobile(phone);
            if (isValid)
            {
                user.Mobile = phone;
            }
            else Console.WriteLine("Invalid phone number.");
        }

        isValid = false;

        while (!isValid)
        {
            Console.WriteLine("Your password must:\r\n- be at least 8 characters long\r\n- contain a number\r\n- contain a lowercase letter\r\n- contain an uppercase letter\r\nPlease enter a password:");
            string input1 = Console.ReadLine()!;
            isValid = _validationService.IsValidPassword(input1);

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
        if (ShouldPromptForLocation())
        {
            isValid = false;

            while (!isValid)
            {
                string input = GetUserInput("location (in the form of X,Y)");
                string[] location = input.Split(',');
                isValid = _validationService.IsValidLocation(location);

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
}