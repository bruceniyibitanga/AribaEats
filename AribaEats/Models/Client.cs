using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models
{
    public class Client : IUser
    {
        public string Id { get ; set ; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public Location Location { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantStyle { get; set; }
        public Location RestaurantLocation { get; set; }
        
        public Client()
        {
        }

        public Client(string name, int age, string email, string phone, string password)
        {
            Name = name;
            Age = age;
            Email = email;
            Mobile = phone;
            Password = password;
        }

        public string GetRestaurantLocation()
        {
            return "IMPLEMENT_LOCATION";
        }
    }
}
