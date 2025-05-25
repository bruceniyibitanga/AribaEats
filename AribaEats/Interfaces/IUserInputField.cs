using AribaEats.Models;
using AribaEats.Services;

namespace AribaEats.Interfaces;

/// <summary>
/// Represents a user input field responsible for collecting and processing user input.
/// </summary>
public interface IUserInputField
{
    void Collect(IUser user, UserValidationService validationService);
}