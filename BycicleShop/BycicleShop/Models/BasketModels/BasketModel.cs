using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BycicleShop.Models.AdminModels;
using BycicleShop.Models.ProductModels;

namespace BycicleShop.Models.BasketModels
{
    public class BasketModel
    {
        [Display(Name = "Total Price")]
        public decimal TotalPrice
        {
            get
            {
                return Items.Sum(item => item.ProductPrice*item.Count);
            }
        }
        [Required]
        public int UserId { get; set; }
        public bool Active { get; set; }
        public List<ProductsCountModel> Items = new List<ProductsCountModel>(); 
    }
}