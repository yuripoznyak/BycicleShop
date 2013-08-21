using System;
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
        private ProductCategoryDataContext _categoryDataContext = new ProductCategoryDataContext();

        public ActionResult Index(int? page)
        {
            var products =
                _productDataContext.Entity.Select(
                    product =>
                    new ProductSimpleModel
                        {
                            Name = product.Name,
                            Price = product.ListPrice,
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
                    Price = p.ListPrice,
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
                _productDataContext.Entity.Where(x => x.Name.Contains(searchString))
                                   .OrderBy(s => s.ProductID)
                                   .Select(
                                       product =>
                                       new ProductSimpleModel
                                           {
                                               Id = product.ProductID,
                                               Name = product.Name,
                                               Price = product.ListPrice
                                           })
                                   .ToList();
            return View(new SearchProductModel { Products = products.ToPagedList(page ?? 1, 20), SearchString = searchString });
        }

        public ActionResult ExtendedSearch(ExtendedSearchModel model, int? page)
        {
            var products =
                _productDataContext.Entity.Where(x => x.Name.Contains(model.SearchString)).OrderBy(s => s.ProductID).ToList();

            if (!String.IsNullOrEmpty(model.Category))
            {
                var id = _categoryDataContext.GetId(model.Category);
                products = products.Where(x => x.ProductCategoryID == id).ToList();
            }
            if (model.MinPrice != null)
            {
                products = products.Where(x => x.ListPrice >= model.MinPrice).ToList();
            }
            if (model.MaxPrice != null && model.MaxPrice != 0)
            {
                products = products.Where(x => x.ListPrice <= model.MaxPrice).ToList();
            }
            var productModel = products.Select(product => new ProductSimpleModel { Id = product.ProductID, Name = product.Name, Price = product.ListPrice }).ToList();
            if (productModel.Count == 0)
            {
                ViewBag.Message = "no elements";
            }
            return
                View(new ExtendedSearchModel
                    {
                        Products = productModel.ToPagedList(page ?? 1, 20),
                        SearchString = model.SearchString,
                        Category = model.Category,
                        MaxPrice = model.MaxPrice,
                        MinPrice = model.MinPrice,
                        Categories = _categoryDataContext.Entity.Select(x => x.Name).ToList()
                    });
        }

        /*public ActionResult ExtendedSearch(int? page, string category, string searchString, int? minPrice, int? maxPrice)
        {
            var products =
                _productDataContext.Entity.Where(x => x.Name.Contains(searchString) && x.ListPrice >= minPrice).OrderBy(s => s.ProductID).ToList();

            if (!String.IsNullOrEmpty(category))
            {
                var id = _categoryDataContext.GetId(category);
                products = products.Where(x => x.ProductCategoryID == id).ToList();
            }
            if (maxPrice != 0)
            {
                products = products.Where(x => x.ListPrice <= maxPrice).ToList();
            }
            var productModel = products.Select(product => new ProductSimpleModel { Id = product.ProductID, Name = product.Name, Price = product.ListPrice }).ToList();
            if (productModel.Count == 0)
            {
                ViewBag.Message = "no elements";
            }
            return
                View(new ExtendedSearchModel
                {
                    Products = productModel.ToPagedList(page ?? 1, 20),
                    SearchString = searchString,
                    Category = category,
                    MaxPrice = maxPrice,
                    MinPrice = minPrice,
                    Categories = _categoryDataContext.Entity.Select(x => x.Name).ToList()
                });
        }*/
    }
}
