using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models
{
    /// <summary>
    /// Represents a customer who uses the platform to place orders.
    /// Implements the <see cref="IUser"/> interface.
    /// </summary>
    public class Customer : IUser
    {
        public string Id { get ; set ; } = Guid.NewGuid().ToString();
        public string Name { get ; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public Location Location { get; set; }

        public Customer()
        {
            
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class with specified details.
        /// </summary>
        /// <param name="name">The name of the customer.</param>
        /// <param name="age">The age of the customer.</param>
        /// <param name="email">The email of the customer.</param>
        /// <param name="phone">The mobile number of the customer.</param>
        /// <param name="password">The password of the customer.</param>
        public Customer(string name, int age, string email, string phone, string password)
        {
            Name = name;
            Age = age;
            Email = email;
            Mobile = phone;
            Password = password;
        }
        
        /// <summary>
        /// A list of orders associated with the customer's purchase history.
        /// </summary>
        private List<Order> _orders = new List<Order>();
        
        /// <summary>
        /// Retrieves the total number of orders made by the customer.
        /// </summary>
        /// <returns>The count of orders placed by the customer.</returns>
        public int GetOrderCount()
        {
            return _orders.Count;
        }
        
        /// <summary>
        /// Calculates the total amount the customer has spent on completed orders.
        /// It sums up orders with the status <see cref="OrderStatus.Ordered"/>.
        /// </summary>
        /// <returns>The total amount spent across completed orders.</returns>
        public decimal GetTotalSpent()
        {
            return _orders
                .Where(order => order.Status == OrderStatus.Ordered) 
                .Sum(order => order.TotalAmount);
        }

        /// <summary>
        /// Adds an order to the customer's purchase history.
        /// </summary>
        /// <param name="order">The order to be added.</param>
        public void AddOrderToPurchaseHistory(Order order)
        {
            _orders.Add(order);
        }
        
        /// <summary>
        /// Retrieves the full order history of the customer.
        /// </summary>
        /// <param name="customer">The customer whose order history is being fetched.</param>
        /// <returns>A list of <see cref="Order"/> objects associated with the customer.</returns>
        public List<Order> GetOrderHistory(Customer customer)
        {
            return _orders;
        }
        
        /// <summary>
        /// Provides a summary of the customer's orders, including the total number of orders
        /// and the total amount spent.
        /// </summary>
        /// <param name="customerId">The ID of the customer for whom the summary is being generated.</param>
        /// <returns>A string summarizing the customer's order history and total spending.</returns>
        public string GetOrderSummary(string customerId)
        {
            int orderCount = GetOrderCount();
            decimal totalSpent = GetTotalSpent();
        
            return $"You've made {orderCount} order(s) and spent a total of ${totalSpent:F2} here.";
        }
        
    }
}
