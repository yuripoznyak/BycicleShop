using System.Linq;
using System.Web.Mvc;
using BycicleShop.Common.SqlContext.Concrete;
using BycicleShop.Models.ProductModels;
using PagedList;

namespace BycicleShop.Controllers
{
    public class StoreManagerController : Controller
    {
        private ProductDataContext _productDataContext = new ProductDataContext();
        
        public ActionResult Index(int? page)
        {
            var products =
                _productDataContext.Entity.Select(
                    product =>
                    new ProductSimpleModel
                        {
                            Name = product.Name,
                            Price = product.StandardCost,
                            Id = product.ProductID
                        }).ToList();

            return View(products.OrderBy(s => s.Id).ToPagedList(page ?? 1, 20));
        }

        public ActionResult ProductInfo(int id)
        {
            var p = _productDataContext.Entity.First(x => x.ProductID == id);
            var product = new ProductExtendedModel
                {
                    Id = p.ProductID,
                    Name = p.Name,
                    Color = p.Color,
                    Price = p.StandardCost,
                    SellEndDate = p.SellEndDate,
                    SellStartDate = p.SellStartDate,
                    Size = p.Size,
                    Weght = p.Weight
                };
            return View(product);
        }

        public ActionResult Search(string searchString, int? page)
        {
            var products =
                 _productDataContext.Entity.Where(x => x.Name.Contains(searchString)).OrderBy(s => s.ProductID).ToList();
            var simpleProducts = products.Select(product => new ProductSimpleModel { Id = product.ProductID, Name = product.Name, Price = product.StandardCost, PhotoName = product.ThumbnailPhotoFileName }).ToList();

            return View(new SearchProductModel { ProductModel = simpleProducts.ToPagedList(page ?? 1, 20), SearchString = searchString });
        }
    }
}
