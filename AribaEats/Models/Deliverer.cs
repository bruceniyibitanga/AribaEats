using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models
{
    public enum DelivererStatus
    {
        Free,
        AcceptedOrder,
        ArrivedAtRestaurant,
        Enroute,
        Delivered
    }
    public class Deliverer : IUser
    {
        public string Id { get ; set ; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public Location Location { get; set; } = new Location(0, 0);
        public DelivererStatus Status { get; set; } = DelivererStatus.Free;
        public string LicencePlate { get; set; }
        List<Order> orders = new List<Order>();
        public Deliverer()
        {
            
        }

        public void UpdateDelivererStatus(Deliverer deliverer)
        {
            switch (deliverer.Status)
            {
                case DelivererStatus.Free:
                    Status = DelivererStatus.AcceptedOrder;
                    break;
                case DelivererStatus.AcceptedOrder:
                    Status = DelivererStatus.ArrivedAtRestaurant;
                    break;
                case DelivererStatus.ArrivedAtRestaurant:
                    Status = DelivererStatus.Enroute;
                    break;
                case DelivererStatus.Enroute:
                    Status = DelivererStatus.Delivered;
                    break;
                case DelivererStatus.Delivered:
                    Status = DelivererStatus.Free;
                    break;
                default:
                    throw new InvalidOperationException("Invalid previous status.");
            }
        }
    }
}
