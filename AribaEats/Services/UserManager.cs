using AribaEats.Helper;
using AribaEats.Interfaces;
using AribaEats.Models;

namespace AribaEats.Services
{
    /// <summary>
    /// Manages user operations such as registration, login, and validation for customers, deliverers, and clients.
    /// </summary>
    public class UserManager
    {
        // Lists to store different types of users
        private List<Customer> _customers = new List<Customer>();
        private List<Deliverer> _deliverers = new List<Deliverer>();
        private List<Client> _clients = new List<Client>();

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email to search for.</param>
        /// <returns>The user with the matching email, or null if not found.</returns>
        public IUser? GetUserByEmail(string email)
        {
            // Combines all user lists and finds the first match by email
            return ((IEnumerable<IUser>)_customers)
                .Concat(_clients)
                .Concat(_deliverers)
                .FirstOrDefault(user => user.Email == email);
        }

        /// <summary>
        /// Checks if an email is not already used by any user.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns><c>true</c> if the email is unique; otherwise, <c>false</c>.</returns>
        public bool IsEmailUnique(string email)
        {
            return !_customers.Any(e => e.Email == email) &&
                   !_deliverers.Any(e => e.Email == email) &&
                   !_clients.Any(e => e.Email == email);
        }

        /// <summary>
        /// Attempts to add a customer if their email is unique.
        /// </summary>
        /// <param name="user">The customer to add.</param>
        /// <returns><c>true</c> if the customer was added; otherwise, <c>false</c>.</returns>
        public bool AddCustomer(Customer user)
        {
            if (!IsEmailUnique(user.Email)) return false;

            _customers.Add(user);
            return true;
        }

        /// <summary>
        /// Verifies if the provided password matches the user's password.
        /// </summary>
        /// <param name="profile">The user profile.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns><c>true</c> if the password matches; otherwise, <c>false</c>.</returns>
        public bool VerifyUserWithPassword(IUser profile, string password)
        {
            return profile.Password == password;
        }

        /// <summary>
        /// Attempts to log in a user with their email and password.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>
        /// A tuple indicating whether the profile exists, whether the password is correct,
        /// and the user object if login is successful.
        /// </returns>
        public (bool ProfileExists, bool PasswordCorrect, IUser? User) Login(string email, string password)
        {
            var profile = GetUserByEmail(email);

            if (profile is not IUser user)
                return (false, false, null); // No user found

            var isCorrectPassword = VerifyUserWithPassword(user, password);

            if (!isCorrectPassword)
                return (true, false, null); // Wrong password
            
            return (true, true, user); // Successful login
        }

        /// <summary>
        /// Attempts to add a deliverer if their email is unique.
        /// </summary>
        /// <param name="deliverer">The deliverer to add.</param>
        /// <returns><c>true</c> if the deliverer was added; otherwise, <c>false</c>.</returns>
        public bool AddDeliverer(Deliverer deliverer)
        {
            if (!IsEmailUnique(deliverer.Email)) return false;

            _deliverers.Add(deliverer);
            return true;
        }

        /// <summary>
        /// Attempts to add a client if their email is unique.
        /// </summary>
        /// <param name="client">The client to add.</param>
        /// <returns><c>true</c> if the client was added; otherwise, <c>false</c>.</returns>
        public bool AddClient(Client client)
        {
            if (!IsEmailUnique(client.Email)) return false;

            _clients.Add(client);
            return true;
        }

        /// <summary>
        /// Logs out the current user and resets their session state.
        /// </summary>
        /// <param name="customerId">The ID of the customer logging out (currently unused).</param>
        public void Logout(string customerId)
        {
            SessionState.ResetVisitedRestaurant(); // Clear session data (e.g., selected restaurant)
        }
    }
}
