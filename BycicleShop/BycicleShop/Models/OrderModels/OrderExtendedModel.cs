using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BycicleShop.Models.AdminModels;
using BycicleShop.Models.ProductModels;

namespace BycicleShop.Models.OrderModels
{
    public class OrderExtendedModel : OrderSimpleModel
    {
        [Required]
        public int OrderId { get; set; }
        
        public bool? Sent { get; set; }
        
        [Display(Name = "Total Price")]
        public decimal? TotalPrice { get; set; }

        public List<ProductsCountModel> Products = new List<ProductsCountModel>();
    }
}