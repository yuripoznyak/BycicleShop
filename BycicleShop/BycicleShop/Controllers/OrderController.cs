using System;
using System.Linq;
using System.Web.Mvc;
using BycicleShop.Common.SqlContext.Concrete;
using BycicleShop.Common.SqlContext.Entities;
using BycicleShop.Models.OrderModels;
using BycicleShop.Models.ProductModels;
using WebMatrix.WebData;

namespace BycicleShop.Controllers
{
    public class OrderController : Controller
    {
        private OrderDataContext _orderDataContext = new OrderDataContext();
        private BasketDataContext _basketDataContext = new BasketDataContext();
        private ProductDataContext _productDataContext = new ProductDataContext();

        public ActionResult MyOrders(int? userId)
        {
            if (userId == null)
            {
                return View();
            }
            var simpleOrders =
                _orderDataContext.GetOrders((int)userId)
                                 .Where(x => x.Sent == true)
                                 .Select(
                                     item =>
                                     new OrderSimpleModel
                                         {
                                             Id = item.OrderId,
                                             Obtained = item.Obtained ?? false,
                                             Adress = item.Adress,
                                             SentDate = item.SentDate
                                         })
                                 .ToList();
            return View(simpleOrders);
        }

        [HttpGet]
        public ActionResult AddToBasket(int productId, string returnUrl)
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            Basket basket = null;
            try
            {
                basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
            }
            catch (InvalidOperationException)
            {
                if (basket == null)
                {
                    CreateBasket(userId);
                    basket = _basketDataContext.Entity.First(x => x.UserId == userId && x.Active);
                }
            }
            basket.ProductsCounts.Add(new ProductsCount{Count = 1, ProductId = _productDataContext.Entity.Find(productId).ProductID});
            _basketDataContext.SaveChanges();
            return !string.IsNullOrEmpty(returnUrl)
               ? Redirect(returnUrl)
               : (ActionResult)RedirectToAction("Index", "Home");
        }

        public ActionResult MyBasket()
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);

            var products =
                _productDataContext.Entity.Where(x => x.ProductsCount.Basket.UserId == userId)
                                   .Select(
                                       item =>
                                       new ProductSimpleModel
                                           {
                                               Id = item.ProductID,
                                               Name = item.Name,
                                               Price = item.StandardCost,
                                               PhotoName = item.ThumbnailPhotoFileName
                                           });

            return View(products.ToList());
        }

        private void CreateBasket(int userId)
        {
            _basketDataContext.AddBasket(userId);
        }
    }
}
