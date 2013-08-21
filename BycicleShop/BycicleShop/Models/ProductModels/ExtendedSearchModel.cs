using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.ProductModels
{
    public class ExtendedSearchModel : SearchProductModel
    {
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Display(Name = "Max price")]
        public decimal? MaxPrice { get; set; }
        [Display(Name = "Min price")]
        public decimal? MinPrice { get; set; }
        
        public ICollection<string> Categories { get; set; }
    }
}