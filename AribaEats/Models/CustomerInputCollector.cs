namespace AribaEats.Models;

public class CustomerInputCollector :BaseUserInputCollector
{
    public CustomerInputCollector(UserValidationService validationService) :base(validationService)
    {
    }

    public Customer CollectCustomerInformation()
    {
        var customer = new Customer();
        CollectBaseUserInfo(customer);

        return customer;
    }
}