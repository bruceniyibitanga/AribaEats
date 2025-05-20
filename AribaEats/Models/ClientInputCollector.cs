using System;
using System.Collections.Generic;
namespace AribaEats.Models;

public class ClientInputCollector : BaseUserInputCollector
{
    public ClientInputCollector(UserValidationService validationService) :base(validationService)
    {
    }

    public Client CollectClientInfo()
    {
        var client = new Client();
        CollectBaseUserInfo(client);
        
        string restaurntName = GetUserInput("restaurant's name");
        string restaurantStyle = RestrauntStyleList();

        return new Client();
    }

    private string RestrauntStyleList()
    {
        List<string> styleList = new List<string>() { "Italian", "French", "Chinese", "Japanese", "American", "Australian" };
        int result;
        bool res = false;

        do
        {
            Console.WriteLine("Choose a restaurant style:");
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

        return styleList[result - 1];
    }
}