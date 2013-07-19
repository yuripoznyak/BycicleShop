using PagedList;

namespace BycicleShop.Models.ProductModels
{
    public class SearchProductModel
    {
        public IPagedList<ProductSimpleModel> ProductModel { get; set; }
        public string SearchString { get;set; }
    }
}