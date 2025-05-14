using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models
{
    public class Deliverer : IUser
    {
        public string Id { get ; set ; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public Location Location { get; set; }

        public Deliverer()
        {
            
        }

        public Deliverer(string name, int age, string email, string phone, string password)
        {
            Name = name;
            Age = age;
            Email = email;
            Mobile = phone;
            Password = password;
        }
    }
}
