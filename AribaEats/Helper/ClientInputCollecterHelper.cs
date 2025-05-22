using AribaEats.Interfaces;
using AribaEats.Services;

namespace AribaEats.Helper;

public class ClientInputCollecterHelper
{
    private RestaurantMenuValidatorService _validatorService;
    protected readonly List<IUserInputField> _inputFields;

    public ClientInputCollecterHelper(RestaurantMenuValidatorService validatorService)
    {
        _validatorService = validatorService;
    }
    
    
}