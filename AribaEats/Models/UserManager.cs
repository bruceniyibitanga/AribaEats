using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models
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
        
        public bool DoesUserExist(string email)
        {
            
            return _customers.Any(e => e.Email == email) | _deliverers.Any(e => e.Email == email) | _clients.Any(e => e.Email == email);
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

        public (bool ProfileExists, bool PasswordCorrect, Customer? User) LoginAsCustomer(string email, string password)
        {
            var profile = GetUserByEmail(email);

            if (profile is not Customer customer)
                return (false, false, null);

            var isCorrectPassword = VerifyUserWithPassword(customer, password);

            if (!isCorrectPassword)
                return (true, false, null);

            currentUser = customer;
            return (true, true, customer);
        }

        public Deliverer LoginAsDeliverer(string email, string password)
        {
            bool isUser = DoesUserExist(email);

            if (!isUser) return new Deliverer();
            
            // Try login user with password
            return _deliverers.SingleOrDefault(e => e.Email == email && e.Password == password);
        }

        public Client LoginAsClient(string email, string password)
        {
            bool isUser = DoesUserExist(email);

            if (!isUser) return new Client();
            
            // Try login user with password
            return _clients.SingleOrDefault(e => e.Email == email && e.Password == password);
        }

        public void Logout()
        {
            currentUser = null;
        }
    }
}
