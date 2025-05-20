using AribaEats.Models.ConsoleFiles;

namespace AribaEats.Interfaces;

public interface IUserRegistrar<T> where T : IUser
{
    void Register(IUser user, MenuNavigator navigator, IMenu redirectTo);
}