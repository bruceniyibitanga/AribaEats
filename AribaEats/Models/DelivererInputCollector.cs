namespace AribaEats.Models;

public class DelivererInputCollector : BaseUserInputCollector
{
    private readonly UserValidationService _validationService;
    public DelivererInputCollector(UserValidationService validationService) :base(validationService)
    {
        _validationService = validationService;
    }

    protected override bool ShouldPromptForLocation() => false;

    public Deliverer CollectDelivererInputInfo()
    {
        var deliverer = new Deliverer();
        CollectBaseUserInfo(deliverer);

        bool isValid = false;
        while (!isValid)
        {
            string licensePlate = GetUserInput("licence plate");
            isValid = _validationService.IsValidLicencePlate(licensePlate);
            if (isValid) deliverer.LicensePlate = licensePlate;
            else System.Console.WriteLine("Invalid license plate");

        }
        return deliverer;
    }
}