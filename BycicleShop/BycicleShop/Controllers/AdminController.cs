using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BycicleShop.Common.SqlContext.Concrete;
using BycicleShop.Common.SqlContext.Entities;
using BycicleShop.Models.AccountModels;
using BycicleShop.Models.AdminModels;
using BycicleShop.Models.OrderModels;
using BycicleShop.Models.ProductModels;
using Itransition.Providers;
using PagedList;

namespace BycicleShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ProductDataContext _productDataContext = new ProductDataContext();
        private readonly ProductCategoryDataContext _productCategoryDataContext = new ProductCategoryDataContext();
        private readonly ProductsCountDataContext _productsCountDataContext = new ProductsCountDataContext();
        private readonly UsersDataContext _usersDataContext = new UsersDataContext();
        private readonly OrderDataContext _orderDataContext = new OrderDataContext();

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
            return
                View(new AddProductModel { Categories = _productCategoryDataContext.Entity.Select(x => x.Name).ToArray() });
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

        public ActionResult UserActivity(string username)
        {
            var user = _usersDataContext.Entity.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var list = user.Orders.Select(order => new OrderStatisticModel
                {
                    OrderId = order.OrderId,
                    Adress = order.Adress,
                    ReceiveDate = order.ReceivedDate,
                    SentDate = order.SentDate,
                    TotalPrice = order.Price,
                    Username = user.UserName
                }).ToList();

            return View(list);
        }

        public ActionResult Order(int id)
        {
            var order = _orderDataContext.Entity.Find(id);
            if (order == null)
            {
                return RedirectToAction("HttpError404", "Error");
            }
            var model = new OrderExtendedModel
            {
                Adress = order.Adress,
                Received = order.Received,
                Sent = order.Sent,
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalPrice = order.Price
            };
            foreach (var product in order.ProductsCounts)
            {
                model.Products.Add(new ProductsCountModel
                {
                    ProductsCountId = product.ProductsCountId,
                    ProductName = product.Product.Name,
                    ProductPrice = product.Product.ListPrice,
                    Count = product.Count,
                    ProductId = product.Product.ProductID,
                    OrderId = product.OrderId
                });
            }
            return View(model);
        }

        public ActionResult Users(int? page)
        {
            var list = _usersDataContext.Entity.ToList().Select(user => new UserProfileModel
                {
                    Id = user.UserId,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }).ToPagedList(page ?? 1, 10);
            return View(list);
        }

        public ActionResult Orders(int? page)
        {
            var orders = _orderDataContext.Entity.ToList().Select(order => new OrderModel
                {
                    Id = order.OrderId,
                    Adress = order.Adress,
                    ReceivedDate = order.ReceivedDate,
                    SentDate = order.SentDate,
                    Username = order.User.UserName
                });
            return View(orders.ToPagedList(page ?? 1, 10));
        }

        [HttpGet]
        public ActionResult AddNewUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewUser(NewUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var membership =
                        (CustomMembershipProvider)
                        Membership.Providers["CustomMembershipProvider"];
                    MembershipCreateStatus createStatus;
                    membership.CreateUser(model.Username, model.Password, model.Email, "", "", true, null, out createStatus);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        return RedirectToAction("Users", "Admin");
                    }
                }
                catch
                {
                    RedirectToAction("HttpError500", "Error");
                }
            }

            return View(model);
        }

        public ActionResult DeleteUser(string id)
        {
            try
            {
                var membership = (CustomMembershipProvider)Membership.Providers["CustomMembershipProvider"];
                if (!membership.DeleteUser(id, true))
                {
                    return RedirectToAction("HttpError500", "Error");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("HttpError500", "Error");
            }
            return RedirectToAction("Users", "Admin");
        }

        public ActionResult UnlockUser(string id)
        {
            try
            {
                var membership = (CustomMembershipProvider)Membership.Providers["CustomMembershipProvider"];
                if (!membership.UnlockUser(id))
                {
                    return RedirectToAction("HttpError500", "Error");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("HttpError500", "Error");
            }
            return RedirectToAction("Index", "Home");

        }
    }
}
