using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models
{
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

        public Customer(string name, int age, string email, string phone, string password)
        {
            Name = name;
            Age = age;
            Email = email;
            Mobile = phone;
            Password = password;
        }
        
        private List<Order> _orders = new List<Order>();
        public void AddOrder(Order order)
        {
            if (order != null && order.CustomerId == Id)
            {
                _orders.Add(order);
            }
        }
        
        public int GetOrderCount()
        {
            return _orders.Count;
        }
        
        public decimal GetTotalSpent()
        {
            return _orders
                .Where(order => order.Status == OrderStatus.Delivered) 
                .Sum(order => order.TotalAmount);
        }

        public void AddOrderToPurchaseHistory(Order order)
        {
            _orders.Add(order);
        }
        
        public string GetOrderSummary(string customerId)
        {
            int orderCount = GetOrderCount();
            decimal totalSpent = GetTotalSpent();
        
            return $"You've made {orderCount} order(s) and spent a total of ${totalSpent:F2} here.";
        }
        
    }
}
