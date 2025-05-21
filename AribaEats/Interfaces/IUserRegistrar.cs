using AribaEats.UI;

namespace AribaEats.Interfaces;

public interface IUserRegistrar<T> where T : IUser
{
    void Register(IUser user, MenuNavigator navigator, IMenu redirectTo);
}