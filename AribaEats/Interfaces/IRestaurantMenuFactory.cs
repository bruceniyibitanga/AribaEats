using AribaEats.Models;
using AribaEats.UI;

namespace AribaEats.Interfaces;

public interface IRestaurantMenuFactory
{
    IMenu CreateRestaurantListMenu(MenuNavigator navigator, Customer customer, string sortOrder);
    IMenu CreateRestaurantSortMenu(MenuNavigator navigator, Customer customer);
}