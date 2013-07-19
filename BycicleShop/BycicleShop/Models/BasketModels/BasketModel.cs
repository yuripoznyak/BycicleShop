using System.Collections.Generic;
using BycicleShop.Models.ProductModels;

namespace BycicleShop.Models.BasketModels
{
    public class BasketModel
    {
        public decimal TotalPrice
        {
            get
            {
                decimal total = 0;
                foreach (var item in Items)
                {
                    total = item.Key.Price*item.Value;
                }
                return total;
            }
        }
        public int UserId { get; set; }
        public bool Active { get; set; }
        public Dictionary<ProductSimpleModel, int> Items = new Dictionary<ProductSimpleModel, int>();

    }
}