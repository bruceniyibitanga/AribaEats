using AribaEats.UI;

namespace AribaEats.Interfaces;

public interface IMenuFactory
{
    IMenu CreateMenuFor(IUser user, MenuNavigator navigator);
    IMenu GetLoginMenu(MenuNavigator navigator);
}