using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Helper;

public class RestaurantMenuInputCollector
{
    private RestaurantMenuValidatorService _validatorService = new();

    public RestaurantMenuItem CollectMenuInfo(Client client)
    {
        bool isValid = false;
        var item = new RestaurantMenuItem();

        while (!isValid)
        {
            Console.WriteLine("Please enter the name of the new item (blank to cancel):");
            var name = Console.ReadLine();
            if (_validatorService.IsValidName(name))
            {
                item.Name = name;
                isValid = true;
            }
            else return new RestaurantMenuItem() { Name = name };
        }
        
        isValid = false;
        
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
            };
        }
        
        
        return item;
    }
}