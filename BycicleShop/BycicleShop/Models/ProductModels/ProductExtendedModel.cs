using System;
using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.ProductModels
{
    public class ProductExtendedModel : ProductSimpleModel
    {
        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Weight")]
        public decimal? Weght { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Sell Start Date")]
        public DateTime SellStartDate { get; set; }

        [Display(Name = "Sell End Date")]
        public DateTime? SellEndDate { get; set; }

        public string PhotoName { get; set; }
    }
}