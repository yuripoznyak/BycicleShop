using System.ComponentModel.DataAnnotations;
using PagedList;

namespace BycicleShop.Models.ProductModels
{
    public class SearchProductModel
    {
        public IPagedList<ProductSimpleModel> Products { get; set; }
        [Required]
        [Display(Name = "Search string")]
        public string SearchString
        {
            get { return _searchString ?? ""; }
            set
            {
                _searchString = value ?? "";
            }
        }

        private string _searchString;
    }
}