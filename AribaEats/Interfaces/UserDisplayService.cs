using AribaEats.Interfaces;

namespace AribaEats.Models;

public class UserDisplayService  : IUserDisplayService
{
    public void DisplayBasicUserInformation(IUser user)
    {
        Console.WriteLine("Your user details are as follows:");
        Console.WriteLine($"Name: {user.Name}");
        Console.WriteLine($"Age: {user.Age}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Mobile: {user.Mobile}");
    }
    
    public void DisplayCustomerInformation(Customer customer)
    {
        DisplayBasicUserInformation(customer);
        Console.WriteLine($"Location: {customer.Location.X},{customer.Location.Y}");
        Console.WriteLine(customer.GetOrderSummary(customer.Id));
    }
    
    public void DisplayDelivererInformation(Deliverer deliverer)
    {
        DisplayBasicUserInformation(deliverer);
        Console.WriteLine($"Licence plate: {deliverer.LicencePlate}");
    }
    
    public void DisplayClientInformation(Client client)
    {
        DisplayBasicUserInformation(client);
        Console.WriteLine($"Restaurant name: {client.Restaurant.Name}");
        Console.WriteLine($"Restaurant style: {client.Restaurant.Style}");
        Console.WriteLine($"Restaurant location: {client.GetRestaurantLocation()}");
    }
}