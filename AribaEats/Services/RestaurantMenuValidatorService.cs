namespace AribaEats.Services;

/// <summary>
/// Provides validation logic for restaurant menu items, such as price and name.
/// </summary>
public class RestaurantMenuValidatorService
{
    /// <summary>
    /// Validates a string input to ensure it represents a valid menu item price.
    /// </summary>
    /// <param name="input">The price as a string.</param>
    /// <param name="price">The parsed decimal price if valid.</param>
    /// <returns>
    /// <c>true</c> if the input is a valid decimal number between 0 and 1000 (exclusive); otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidPrice(string input, out decimal price)
    {
        // Try to convert input to decimal and check it's in the valid range
        if (decimal.TryParse(input, out price) && price is > 0 and < 1000) return true;

        return false;
    }

    /// <summary>
    /// Checks if the provided menu item name is valid.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <returns>
    /// <c>true</c> if the name is not null, empty, or whitespace; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidName(string name)
    {
        // Ensure the name is not empty or just whitespace
        if (!string.IsNullOrWhiteSpace(name)) return true;

        return false;
    }
}
