using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.ProductModels
{
    public class ProductSimpleModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }
    }
}