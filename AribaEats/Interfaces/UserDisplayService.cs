using AribaEats.Interfaces;

namespace AribaEats.Models;

/// <summary>
/// Service responsible for displaying user information in a structured format.
/// Implements the <see cref="IUserDisplayService"/> interface.
/// </summary>
public class UserDisplayService : IUserDisplayService
{
    /// <summary>
    /// Displays basic information of a user, such as their name, age, email, and mobile number.
    /// </summary>
    /// <param name="user">The user whose information needs to be displayed.</param>
    public void DisplayBasicUserInformation(IUser user)
    {
        Console.WriteLine("Your user details are as follows:");
        Console.WriteLine($"Name: {user.Name}");
        Console.WriteLine($"Age: {user.Age}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Mobile: {user.Mobile}");
    }
    
    /// <summary>
    /// Displays specific details of a customer, including their location and order summary.
    /// </summary>
    /// <param name="customer">The customer whose information needs to be displayed.</param>
    public void DisplayCustomerInformation(Customer customer)
    {
        DisplayBasicUserInformation(customer);
        Console.WriteLine($"Location: {customer.Location.X},{customer.Location.Y}");
        Console.WriteLine(customer.GetOrderSummary(customer.Id));
    }
    
    /// <summary>
    /// Displays specific details of a deliverer, including their vehicle license plate.
    /// </summary>
    /// <param name="deliverer">The deliverer whose information needs to be displayed.</param>
    public void DisplayDelivererInformation(Deliverer deliverer)
    {
        DisplayBasicUserInformation(deliverer);
        Console.WriteLine($"Licence plate: {deliverer.LicencePlate}");
    }
    
    /// <summary>
    /// Displays specific details of a client, including their restaurant's name, style, and location.
    /// </summary>
    /// <param name="client">The client whose information needs to be displayed.</param>
    public void DisplayClientInformation(Client client)
    {
        DisplayBasicUserInformation(client);
        Console.WriteLine($"Restaurant name: {client.Restaurant.Name}");
        Console.WriteLine($"Restaurant style: {client.Restaurant.Style}");
        Console.WriteLine($"Restaurant location: {client.GetRestaurantLocation()}");
    }
}