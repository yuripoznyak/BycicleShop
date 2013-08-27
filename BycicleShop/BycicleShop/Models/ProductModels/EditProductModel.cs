using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BycicleShop.Models.ProductModels
{
    public class EditProductModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(25)]
        [Display(Name = "Product Number")]
        public string ProductNumber { get; set; }

        [MaxLength(15)]
        [Display(Name = "Color")]
        public string Color { get; set; }

        [Required]
        [Display(Name = "Standard Cost")]
        public decimal StandardCost { get; set; }

        [Required]
        [Display(Name = "List Price")]
        public decimal ListPrice { get; set; }

        [MaxLength(5)]
        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Weight")]
        public decimal? Weight { get; set; }

        [Display(Name = "Thumb Nail Photo")]
        public HttpPostedFileBase Photo { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        public IEnumerable<string> Categories { get; set; }
    }
}