using System.Text.RegularExpressions;

namespace AribaEats.Models;

public class UserValidationService
{
    private readonly UserManager _userManager;
        
    public UserValidationService(UserManager userManager)
    {
        _userManager = userManager;
    }
    
    public bool IsValidNameInput(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(name, @"^[a-zA-Z\s'-]+$") || !Regex.IsMatch(name, @"[a-zA-Z]")) return false;

        return true;
    }

    public bool IsValidAge(string input)
    {
        if (int.TryParse(input, out int age))
        {
            return age >= 18 && age <= 100;
        }
        return false;
    }

    public bool IsValidMobile(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.Length != 10)
            return false;

        if (input[0] != '0')
            return false;

        // Check if all characters are digits
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
                return false;
        }

        return true;
    }

    public bool IsValidEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        int atIndex = input.IndexOf('@');
        int lastAtIndex = input.LastIndexOf('@');

        // Check exactly one @
        if (atIndex == -1 || atIndex != lastAtIndex)
            return false;

        // Check at least one character before and after @
        if (atIndex == 0 || atIndex == input.Length - 1)
            return false;

        return true;
    }

    public bool IsEmailUnique(string email)
    {
       return _userManager.IsEmailUnique(email);
    }

    public bool IsValidPassword(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        // Regex to check at least one lowercase, one uppercase, one digit, and minimum length of 8
        return Regex.IsMatch(input, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
    }

    public bool IsValidLocation(string[] location)
    {
        if (location.Length != 2) return false;

        foreach (string value in location)
        {
            bool canParse = int.TryParse(value, out int result);

            if (!canParse) return false;
        }

        return true;
    }

    public bool IsValidLicencePlate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Regex to check length and allowed characters (uppercase letters, numbers, and spaces)
        return Regex.IsMatch(input, @"^[A-Z0-9 ]{1,8}$") && !string.IsNullOrWhiteSpace(input);
    }
}