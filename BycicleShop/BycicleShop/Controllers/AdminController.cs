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
        private ProductDataContext _productDataContext = new ProductDataContext();
        private ProductCategoryDataContext _productCategoryDataContext = new ProductCategoryDataContext();


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id, string returnUrl)
        {
            _productDataContext.Entity.Remove(_productDataContext.Entity.FirstOrDefault(x => x.ProductID == id));
            return !string.IsNullOrEmpty(returnUrl)
               ? Redirect(returnUrl)
               : (ActionResult)RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(AddProductModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var categoryId =
                    _productCategoryDataContext.Entity.FirstOrDefault(x => x.Name == model.Category).ProductCategoryID;
                byte[] photo;
                using (var ms = new MemoryStream())
                {
                    model.Photo.InputStream.CopyTo(ms);
                    photo = ms.GetBuffer();
                }

                _productDataContext.Entity.Add(new Product
                    {
                        Name = model.Name,
                        ProductNumber = model.ProductNumber,
                        Color = model.Color,
                        Weight = model.Weight,
                        Size = model.Size,
                        StandardCost = model.StandardConst,
                        ListPrice = model.ListPrice,
                        SellStartDate = DateTime.UtcNow,
                        ProductCategoryID = categoryId,
                        ThumbnailPhotoFileName = Path.GetFileName(model.Photo.FileName),
                        ThumbNailPhoto = photo
                    });
                _productDataContext.SaveChanges();
                return !string.IsNullOrEmpty(returnUrl)
                           ? Redirect(returnUrl)
                           : (ActionResult)RedirectToAction("Index", "Home");
            }
            return View();
        }

    }
}
