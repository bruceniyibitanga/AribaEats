using AribaEats.Models;

namespace AribaEats.Interfaces;

public interface IUserDisplayService
{
    void DisplayBasicUserInformation(IUser user);
    void DisplayCustomerInformation(Customer customer);
    void DisplayDelivererInformation(Deliverer deliverer);
    void DisplayClientInformation(Client client);
}