using System.Collections.Generic;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Models.OrderModels
{
    public class OrderExtendedModel
    {
        public int OrderId { get; set; }
        public string Adress { get; set; }
        public User User { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public bool Sent { get; set; }
        public bool Obtained { get; set; }
    }
}