using System.Text.RegularExpressions;
namespace AribaEats.Services;


/// <summary>
/// Provides validation methods for user-related input fields.
/// </summary>
public class UserValidationService
{
    private readonly UserManager _userManager;

    /// <summary>
    /// Initialises a new instance of the <see cref="UserValidationService"/> class.
    /// </summary>
    /// <param name="userManager">A user manager used for user-related checks (e.g., email uniqueness).</param>
    public UserValidationService(UserManager userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Validates a user's name input.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <returns><c>true</c> if the name is valid; otherwise, <c>false</c>.</returns>
    public bool IsValidNameInput(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || 
            !Regex.IsMatch(name, @"^[a-zA-Z\s'-]+$") || 
            !Regex.IsMatch(name, @"[a-zA-Z]")) return false;

        return true;
    }

    /// <summary>
    /// Validates a user's age input.
    /// </summary>
    /// <param name="input">The age as a string.</param>
    /// <returns><c>true</c> if the input is a valid age between 18 and 100; otherwise, <c>false</c>.</returns>
    public bool IsValidAge(string input)
    {
        if (int.TryParse(input, out int age))
        {
            return age >= 18 && age <= 100;
        }
        return false;
    }

    /// <summary>
    /// Validates a user's mobile number.
    /// </summary>
    /// <param name="input">The mobile number as a string.</param>
    /// <returns><c>true</c> if the mobile number is valid; otherwise, <c>false</c>.</returns>
    public bool IsValidMobile(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.Length != 10)
            return false;

        if (input[0] != '0')
            return false;

        foreach (char c in input)
        {
            if (!char.IsDigit(c))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Validates an email format.
    /// </summary>
    /// <param name="input">The email address as a string.</param>
    /// <returns><c>true</c> if the email format is valid; otherwise, <c>false</c>.</returns>
    public bool IsValidEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        int atIndex = input.IndexOf('@');
        int lastAtIndex = input.LastIndexOf('@');

        if (atIndex == -1 || atIndex != lastAtIndex)
            return false;

        if (atIndex == 0 || atIndex == input.Length - 1)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if an email address is unique.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns><c>true</c> if the email is unique; otherwise, <c>false</c>.</returns>
    public bool IsEmailUnique(string email)
    {
       return _userManager.IsEmailUnique(email);
    }

    /// <summary>
    /// Validates a password for strength and security.
    /// </summary>
    /// <param name="input">The password as a string.</param>
    /// <returns><c>true</c> if the password is valid; otherwise, <c>false</c>.</returns>
    public bool IsValidPassword(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        return Regex.IsMatch(input, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
    }

    /// <summary>
    /// Validates a location represented by an array of two string values.
    /// </summary>
    /// <param name="location">An array containing two string values (e.g., coordinates).</param>
    /// <returns><c>true</c> if both values can be parsed as integers; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Validates a vehicle licence plate format.
    /// </summary>
    /// <param name="input">The licence plate as a string.</param>
    /// <returns><c>true</c> if the licence plate format is valid; otherwise, <c>false</c>.</returns>
    public bool IsValidLicencePlate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return Regex.IsMatch(input, @"^[A-Z0-9 ]{1,8}$") && !string.IsNullOrWhiteSpace(input);
    }

    /// <summary>
    /// Validates a restaurant name.
    /// </summary>
    /// <param name="input">The restaurant name as a string.</param>
    /// <returns><c>true</c> if the name is not empty or whitespace; otherwise, <c>false</c>.</returns>
    public bool IsValidRestaurantName(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return true;
    }
}
