namespace AribaEats.Models;

public class ClientInputCollector : BaseUserInputCollector
{
    public ClientInputCollector(UserValidationService validationService) : base(validationService)
    {
    }
    protected override void ConfigureInputFields()
    {
        _inputFields.Add(new NameInputField(() => GetUserInput("name")));
        _inputFields.Add(new AgeInputField(() => GetUserInput("age (18-100)")));
        _inputFields.Add(new EmailInputField(() => GetUserInput("email address")));
        _inputFields.Add(new MobileInputField(() => GetUserInput("mobile phone number")));
        _inputFields.Add(new PasswordInputField(() => GetUserInput("password")));
        _inputFields.Add(new RestaurantNameInputField(() => GetUserInput("restaurant's name")));
        _inputFields.Add(new RestaurantStyleInputField());
        _inputFields.Add(new LocationInputField(() => GetUserInput("location (in the form of X,Y)")));
    }

    public Client CollectClientInfo()
    {
        var client = new Client();
        CollectUserInputInfo(client);
        return client;
    }

    // private string CollectRestaurantStyle()
    // {
    //     List<string> styleList = new List<string> { "Italian", "French", "Chinese", "Japanese", "American", "Australian" };
    //     int result;
    //     bool res = false;
    //     do
    //     {
    //         Console.WriteLine("Please select your restaurant's style:");
    //         for (int i = 0; i < styleList.Count; i++)
    //         {
    //             Console.WriteLine($"{i + 1}: {styleList[i]}");
    //         }
    //         Console.WriteLine($"Please enter a choice between 1 and {styleList.Count}:");
    //         res = int.TryParse(Console.ReadLine(), out result);
    //         // Validate the input is within the correct range
    //         if (!res || result < 1 || result > styleList.Count)
    //         {
    //             Console.WriteLine("Invalid input. Please try again.");
    //             res = false;
    //         }
    //     } while (!res);
    //     return styleList[result - 1];
    // }
}