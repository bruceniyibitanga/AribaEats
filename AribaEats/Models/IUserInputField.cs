using AribaEats.Interfaces;

namespace AribaEats.Models;

public interface IUserInputField
{
    void Collect(IUser user, UserValidationService validationService);
}