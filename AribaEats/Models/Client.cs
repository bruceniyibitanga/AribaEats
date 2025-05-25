using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models
{
    /// <summary>
    /// Represents a client who operates a restaurant on the platform.
    /// Implements the <see cref="IUser"/> interface.
    /// </summary>
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
        
        public string GetRestaurantLocation()
        {
            return $"{Location.X},{Location.Y}";
        }

        public List<RestaurantMenuItem> GetMenu()
        {
            return Restaurant.MenuItems;
        }
    }
}
