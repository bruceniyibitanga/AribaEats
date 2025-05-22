using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;

namespace AribaEats.Services
{
    public class UserManager
    {
        private IUser currentUser = null;  
        private List<Customer> _customers = new List<Customer>();
        private List<Deliverer> _deliverers = new List<Deliverer>();
        private List<Client> _clients = new List<Client>();

        public IUser? GetUserByEmail(string email)
        {
            return ((IEnumerable<IUser>)_customers)
                .Concat(_clients)
                .Concat(_deliverers)
                .FirstOrDefault(user => user.Email == email);
        }
        public bool IsEmailUnique(string email)
        {
            return !_customers.Any(e => e.Email == email) && !_deliverers.Any(e => e.Email == email) && !_clients.Any(e => e.Email == email);
        }

        public bool AddCustomer(Customer user)
        {
            if (!IsEmailUnique(user.Email)) return false;
            _customers.Add(user);
            return true;
        }

        public bool VerifyUserWithPassword(IUser profile, string password)
        {
            return profile.Password == password;
        }

        public (bool ProfileExists, bool PasswordCorrect, IUser? User) Login(string email, string password)
        {
            var profile = GetUserByEmail(email);

            if (profile is not IUser customer)
                return (false, false, null);

            var isCorrectPassword = VerifyUserWithPassword(customer, password);

            if (!isCorrectPassword)
                return (true, false, null);

            currentUser = customer;
            return (true, true, customer);
        }

        public bool AddDeliverer(Deliverer deliverer)
        {
            if (!IsEmailUnique(deliverer.Email)) return false;
            _deliverers.Add(deliverer);
            return true;
        }

        public bool AddClient(Client client)
        {
            if (!IsEmailUnique(client.Email)) return false;
            _clients.Add(client);
            return true;
        }

        public void Logout()
        {
            SessionState.HasVisitedOrderScreen = false;
            currentUser = null;
        }
    }
}
