using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.ProductModels
{
    public class ProductsCountModel
    {
        [Required]
        public int ProductsCountId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Count { get; set; }
        public int? OrderId { get; set; }
        [Required]
        public decimal ProductPrice { get; set; }
        [Required]
        public string ProductName { get; set; }
    }
}