namespace AribaEats.Services;

public class RestaurantMenuValidatorService
{
    public bool IsValidPrice(string input, out decimal price)
    {
        if (decimal.TryParse(input, out price) && price is > 0 and < 1000) return true;
        return false;
    }

    public bool IsValidName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name)) return true;
        return false;
    }
}