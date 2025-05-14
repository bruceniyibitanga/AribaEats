using AribaEats.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Interfaces
{
    public interface IUser
    {
        string Id { get ; set ; }
        string Name { get; set; }
        int Age { get; set; }
        string Email { get; set; }
        string Mobile { get; set; }
        string Password { get; set; }
        Location Location { get; set; }

    }
}
