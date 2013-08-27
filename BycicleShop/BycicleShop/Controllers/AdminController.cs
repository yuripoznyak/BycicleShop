using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BycicleShop.Common.SqlContext.Concrete;
using BycicleShop.Common.SqlContext.Entities;
using BycicleShop.Models.ProductModels;

namespace BycicleShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ProductDataContext _productDataContext = new ProductDataContext();
        private readonly ProductCategoryDataContext _productCategoryDataContext = new ProductCategoryDataContext();
        private readonly ProductsCountDataContext _productsCountDataContext = new ProductsCountDataContext();

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult DeleteProduct(int id)
        {
            _productDataContext.Entity.Remove(_productDataContext.Entity.FirstOrDefault(x => x.ProductID == id));
            _productDataContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            return View(new AddProductModel{Categories = _productCategoryDataContext.Entity.Select(x => x.Name).ToArray()});
        }

        [HttpPost]
        public ActionResult AddProduct(AddProductModel model)
        {
            if (ModelState.IsValid)
            {
                var categoryId =
                    _productCategoryDataContext.Entity.First(x => x.Name == model.Category).ProductCategoryID;
                byte[] photo = null;
                string filename = null;
                using (var ms = new MemoryStream())
                {
                    if (model.Photo != null)
                    {
                        model.Photo.InputStream.CopyTo(ms);
                        photo = ms.GetBuffer();
                        filename = Path.GetFileName(model.Photo.FileName);
                    }
                }

                _productDataContext.Entity.Add(new Product
                    {
                        Name = model.Name,
                        ProductNumber = model.ProductNumber,
                        Color = model.Color,
                        Weight = model.Weight,
                        Size = model.Size,
                        StandardCost = model.StandardCost,
                        ListPrice = model.ListPrice,
                        SellStartDate = DateTime.UtcNow,
                        ProductCategoryID = categoryId,
                        ThumbnailPhotoFileName = filename,
                        ThumbNailPhoto = photo,
                        ModifiedDate = DateTime.Now
                    });
                _productDataContext.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            var product = _productDataContext.Entity.Find(id);
            if (product == null) return RedirectToAction("Index", "Home");
            var model = new EditProductModel
                {
                    Id = product.ProductID,
                    Name = product.Name,
                    Categories = _productCategoryDataContext.Entity.Select(x => x.Name).ToList(),
                    Category = _productCategoryDataContext.Entity.Find(product.ProductCategoryID).Name,
                    Color = product.Color,
                    ListPrice = product.ListPrice,
                    ProductNumber = product.ProductNumber,
                    Size = product.Size,
                    StandardCost = product.StandardCost,
                    Weight = product.Weight
                };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditProduct(EditProductModel model)
        {
            if (ModelState.IsValid)
            {
                var product = _productDataContext.Entity.Find(model.Id);
                product.Name = model.Name;
                product.ProductCategoryID =
                    _productCategoryDataContext.Entity.First(x => x.Name == model.Category).ProductCategoryID;
                product.ListPrice = model.ListPrice;
                product.Color = model.Color;
                product.ModifiedDate = DateTime.Now;
                product.StandardCost = model.StandardCost;
                product.Weight = model.Weight;
                product.Size = model.Size;
                product.ProductNumber = model.ProductNumber;
                /*byte[] photo = null;
                string filename = null;
                using (var ms = new MemoryStream())
                {
                    if (model.Photo != null)
                    {
                        model.Photo.InputStream.CopyTo(ms);
                        photo = ms.GetBuffer();
                        filename = Path.GetFileName(model.Photo.FileName);
                    }
                }
                product.ThumbNailPhoto = photo;
                product.ThumbnailPhotoFileName = filename;*/
                _productDataContext.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

       public ActionResult ProductStatistic(int id)
       {
           var products = _productsCountDataContext.Entity.Where(x => x.ProductId == id && x.OrderId != null);
           var list = products.Select(product => new ProductStatisticModel
               {
                   Count = product.Count,
                   ProductId = product.ProductId,
                   ProductName = product.Product.Name,
                   ReceiveDate = product.Order.ReceivedDate,
                   SentDate = product.Order.ReceivedDate,
                   UserName = product.Order.User.UserName
               }).ToList();
           return View(list);
       }
    }
}
