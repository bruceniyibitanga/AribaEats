using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models
{
    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Location(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
