using AribaEats.Interfaces;
using AribaEats.Models.ConsoleFiles;

namespace AribaEats.Models;

public interface IMenuFactory
{
    IMenu CreateMenuFor(IUser user, MenuNavigator navigator);
    IMenu GetLoginMenu(MenuNavigator navigator);
}