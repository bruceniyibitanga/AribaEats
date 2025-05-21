using AribaEats.Interfaces;

namespace AribaEats.Models;

public interface IUserDisplayService
{
    void DisplayBasicUserInformation(IUser user);
    void DisplayCustomerInformation(Customer customer);
    void DisplayDelivererInformation(Deliverer deliverer);
    void DisplayClientInformation(Client client);
}