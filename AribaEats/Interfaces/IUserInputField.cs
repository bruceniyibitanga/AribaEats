using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Interfaces;

public interface IUserInputField
{
    void Collect(IUser user, UserValidationService validationService);
}