using AribaEats.UI;

namespace AribaEats.Interfaces;

/// <summary>
/// Represents a service responsible for user registration.
/// This generic interface allows registration of any user type that implements the <see cref="IUser"/> interface.
/// </summary>
/// <typeparam name="T">The type of user being registered, constrained to types implementing <see cref="IUser"/>.</typeparam>
public interface IUserRegistrar<T> where T : IUser
{
    void Register(IUser user, MenuNavigator navigator, IMenu redirectTo);
}