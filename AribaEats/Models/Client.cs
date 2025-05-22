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
        public string Id { get ; set ; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public Restaurant Restaurant { get; set; } = new Restaurant();
        public Location Location { get; set; }
        
        public Client()
        {
        }

        public Client(string name, int age, string email, string phone, string password, string id, Location location)
        {
            Name = name;
            Age = age;
            Email = email;
            Mobile = phone;
            Password = password;
            Location = location;
            Restaurant.Location = location;
        }

        public string GetRestaurantLocation()
        {
            return $"{Location.X},{Location.Y}";
        }
        
        public void AddItemToMenu(RestaurantMenuItem item)
        {
            Restaurant.MenuItems.Add(item);
        }

        public List<RestaurantMenuItem> GetMenu()
        {
            return Restaurant.MenuItems;
        }
    }
}
