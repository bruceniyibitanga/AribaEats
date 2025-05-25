using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

/// <summary>
/// Responsible for collecting restaurant menu item details from user input.
/// Provides a user interface to gather and validate item details such as name and price.
/// </summary>
public class RestaurantMenuInputCollector
{
    /// <summary>
    /// A validation service used to ensure that the menu item information provided by the user is valid.
    /// </summary>
    private RestaurantMenuValidatorService _validatorService = new();

    /// <summary>
    /// Collects information about a new restaurant menu item by interacting with the user.
    /// Validates input such as item name and price during the process.
    /// </summary>
    /// <param name="client">The client instance requesting the menu item to be added (potentially for context-specific use).</param>
    /// <returns>
    /// A <see cref="RestaurantMenuItem"/> instance containing valid name and price values 
    /// provided by the user. If the user enters invalid inputs, default values may be returned.
    /// </returns>
    public RestaurantMenuItem CollectMenuInfo(Client client)
    {
        bool isValid = false;
        var item = new RestaurantMenuItem();

        // Loop to collect and validate the item name
        while (!isValid)
        {
            Console.WriteLine("Please enter the name of the new item (blank to cancel):");
            var name = Console.ReadLine();
            if (_validatorService.IsValidName(name))
            {
                item.Name = name;
                isValid = true;
            }
            else return new RestaurantMenuItem() { Name = name }; // Default item returned on invalid input
        }
        
        // Reset validation for the next input
        isValid = false; 

        // Loop to collect and validate the price
        while (!isValid)
        {
            Console.WriteLine("Please enter the price of the new item (without the $):");
            var res = Console.ReadLine();
            if (_validatorService.IsValidPrice(res, out var price))
            {
                item.Price = price;
                isValid = true;
            }
            else
            {
                Console.WriteLine("Invalid price.");
                isValid = false;
            }
        }
        
        return item;
    }
}