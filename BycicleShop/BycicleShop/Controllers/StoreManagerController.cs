using System;
using System.Collections.Generic;
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

        public ActionResult Index()
        {
            /*var categories =
                _categoryDataContext.Entity.Where(x => x.ParentProductCategoryID == null).Select(x => new CategoryModel
                    {
                        Id = x.ProductCategoryID,
                        Name = x.Name,
                        //PhotoId = _productDataContext.Entity.First(s => s.ProductCategory.ParentProductCategoryID == x.ParentProductCategoryID).ProductID
                    }).ToList();*/

            //var items = _categoryDataContext.Entity.Where(x => x.ParentProductCategoryID == null);
            var categories = new List<CategoryModel>();
            foreach (var productCategory in _categoryDataContext.Entity.Where(x => x.ParentProductCategoryID == null))
            {
                categories.Add(new CategoryModel
                    {
                        Id = productCategory.ProductCategoryID,
                        Name = productCategory.Name,
                        PhotoId = _productDataContext.Entity.First(s => s.ProductCategory.ParentProductCategoryID == productCategory.ProductCategoryID).ProductID
                    });
            }

            return View(categories);
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
                products = products.Where(x => x.ProductCategoryID == id || x.ProductCategory.ParentProductCategoryID == id).ToList();
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

        public ActionResult Products(int id, int? page)
        {
            var products =
                _productDataContext.Entity.Where(
                    x => x.ProductCategory.ProductCategoryID == id || x.ProductCategory.ParentProductCategoryID == id)
                                   .Select(
                                       product =>
                                       new ProductSimpleModel
                                           {
                                               Name = product.Name,
                                               Price = product.ListPrice,
                                               Id = product.ProductID
                                           }).ToList();

            return View(products.OrderBy(s => s.Id).ToPagedList(page ?? 1, 20));
        }
    }
}
