using System;
using System.Linq;
using System.Web.Mvc;
using BycicleShop.Common.SqlContext.Concrete;
using BycicleShop.Common.SqlContext.Entities;
using BycicleShop.Models.BasketModels;
using BycicleShop.Models.OrderModels;
using BycicleShop.Models.ProductModels;
using Recaptcha;
using WebMatrix.WebData;

namespace BycicleShop.Controllers
{
    public class OrderController : Controller
    {
        private OrderDataContext _orderDataContext = new OrderDataContext();
        private BasketDataContext _basketDataContext = new BasketDataContext();
        private ProductsCountDataContext _productsCountDataContext = new ProductsCountDataContext();
        private UsersDataContext _usersDataContext = new UsersDataContext();

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult MyOrders()
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            if (userId == -1)
            {
                return RedirectToAction("Login", "Account");
            }
            var simpleOrders =
                _orderDataContext.GetOrders(userId)
                                 .Where(x => x.Sent == true)
                                 .Select(
                                     item =>
                                     new OrderSimpleModel
                                         {
                                             Id = item.OrderId,
                                             Received = item.Received,
                                             Adress = item.Adress,
                                             SentDate = item.SentDate
                                         })
                                 .ToList();
            return View(simpleOrders);
        }
        
        public ActionResult Order(int id)
        {
            var order = _usersDataContext.Entity.First(x => x.UserName == User.Identity.Name).Orders.First(x => x.OrderId == id);
            //var order = _orderDataContext.Entity.Find(id);
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

        [HttpGet]
        public ActionResult MakeOrderReceived(int id)
        {
            var model = new OrderReceivedModel
                {
                    OrderId = id
                };
            return View(model);
        }

        public ActionResult MakeOrderReceived(OrderReceivedModel model)
        {
            var order = _orderDataContext.Entity.Find(model.OrderId);
            order.Review = model.Review;
            order.Received = true;
            order.ReceivedDate = DateTime.UtcNow;
            _orderDataContext.SaveChanges();
            return RedirectToAction("Order", new {id = model.OrderId});
        }

        public ActionResult AddToBasket(int productId, string returnUrl, int count)
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            if (userId == -1)
            {
                return RedirectToAction("Login", "Account");
            }
            Basket basket = null;
            try
            {
                basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            }
            catch
            {
                if (basket == null)
                {
                    _basketDataContext.AddBasket(userId);
                    basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
                }
                else
                {
                    return RedirectToAction("MyBasket");
                }
            }
            if (!_productsCountDataContext.AddItem(basket.BasketId, productId, count))
            {
                return RedirectToAction("Index", "Home");
            }
            
            return !string.IsNullOrEmpty(returnUrl)
               ? Redirect(returnUrl)
               : (ActionResult)RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult MyBasket()
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            if (userId == -1)
            {
                return RedirectToAction("Login", "Account");
            }
            Basket basket;
            try
            {
                basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            }
            catch
            {
                _basketDataContext.AddBasket(userId);
                basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            }
            if (basket == null)
            {
                _basketDataContext.AddBasket(userId);
                basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            }
            if (basket.ProductsCounts.Count == 0)
            {
                return RedirectToAction("Index", "StoreManager");
            }

            var basketModel = new BasketModel { Active = basket.Active, UserId = basket.UserId };
            foreach (var product in basket.ProductsCounts)
            {
                basketModel.Items.Add(new ProductsCountModel
                {
                    ProductsCountId = product.ProductsCountId,
                    ProductName = product.Product.Name,
                    ProductPrice = product.Product.ListPrice,
                    Count = product.Count,
                    ProductId = product.Product.ProductID,
                    OrderId = product.OrderId
                });
            }
            return View(basketModel);
        }
        
        public ActionResult RemoveItemFromBasket(int productsCountId)
        {
            _productsCountDataContext.Entity.Remove(_productsCountDataContext.Entity.Find(productsCountId));
            _productsCountDataContext.SaveChanges();
            return RedirectToAction("MyBasket");
        }

        public ActionResult CheckoutStepOne()
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            if (userId < 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            if (basket == null)
            {
                _basketDataContext.AddBasket(userId);
                basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            }
            if (basket.ProductsCounts.Count == 0)
            {
                return RedirectToAction("Index", "StoreManager");
            }

            var basketModel = new BasketModel
                {
                    Active = basket.Active,
                    UserId = basket.UserId
                };
            foreach (var product in basket.ProductsCounts)
            {
                basketModel.Items.Add(new ProductsCountModel()
                {
                    ProductsCountId = product.ProductsCountId,
                    ProductName = product.Product.Name,
                    ProductPrice = product.Product.ListPrice,
                    Count = product.Count,
                    ProductId = product.Product.ProductID,
                    OrderId = product.OrderId
                });
            }
            
            return View(basketModel);
        }

        [HttpGet]
        public ActionResult CheckoutStepTwo(decimal price)
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            if (userId < 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            if (basket == null)
            {
                _basketDataContext.AddBasket(userId);
                basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            }
            if (basket.ProductsCounts.Count == 0)
            {
                return RedirectToAction("Index", "StoreManager");
            }

            var model = new OrderStepModel
                {
                    Adress = basket.User.Adress,
                    BasketId = basket.BasketId,
                    TotalPrice = price
                };
            return View(model);
        }

        [HttpPost]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult CheckoutStepTwo(OrderStepModel model, bool captchaValid, string captchaErrorMessage)
        {
            if (ModelState.IsValid)
            {
                if (!captchaValid)
                {
                    ModelState.AddModelError("recaptcha", captchaErrorMessage);
                    return View(model);
                }
                var name = User.Identity.Name;
                var userId = WebSecurity.GetUserId(name);
                if (userId < 0)
                {
                    return RedirectToAction("Login", "Account");
                }
                var order = new Order
                    {
                        UserId = userId,
                        SentDate = DateTime.UtcNow,
                        Sent = true,
                        Price = model.TotalPrice,
                        PhoneNumber = model.PhoneNumber,
                        Received = false,
                        ReceivedDate = null,
                        Comments = model.Comments,
                        Adress = model.Adress
                    };
                if (_orderDataContext.AddOrder(order))
                {
                    var basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
                    basket.Active = false;
                    _basketDataContext.SaveChanges();
                    var orderId = _orderDataContext.GetLastOrderId(userId);
                    if (orderId == null)
                    {
                        return RedirectToAction("CheckoutStepOne");
                    }
                    /*foreach (var product in _productsCountDataContext.Entity.Where(x => x.BasketId == model.BasketId))
                    {
                        product.OrderId = orderId;
                    }*/
                    var products = _productsCountDataContext.Entity.Where(x => x.BasketId == model.BasketId);
                    foreach (var productsCount in products)
                    {
                        productsCount.OrderId = orderId;
                    }
                    _productsCountDataContext.SaveChanges();
                    return RedirectToAction("CheckoutStepThree");
                }
                return RedirectToAction("CheckoutStepOne");
            }
            return View(model);
        }

        public ActionResult CheckoutStepThree()
        {
            return View();
        }
        
        public ActionResult ChangeProductsCount(int productsCountId, int count)
        {
            if (count <= 0)
            {
                return RedirectToAction("MyBasket");
            }

            try
            {
                var item = _productsCountDataContext.Entity.First(x => x.ProductsCountId ==productsCountId);
                item.Count = count;
                _productsCountDataContext.SaveChanges();
                return RedirectToAction("MyBasket");
            }
            catch
            {
                return RedirectToAction("MyBasket");
            }
        }
    }
}
