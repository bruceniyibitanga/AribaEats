namespace AribaEats.Services;

public abstract class OrderInputValidator
{
    public bool IsValidQuantity(int quantity)
    {
        return int.TryParse(quantity.ToString(), out _);
    }
}