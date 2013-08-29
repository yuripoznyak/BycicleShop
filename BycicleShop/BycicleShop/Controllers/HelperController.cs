using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BycicleShop.Common.SqlContext.Concrete;
using Newtonsoft.Json;

namespace BycicleShop.Controllers
{
    public class HelperController : Controller
    {
        private ProductDataContext _productDataContext = new ProductDataContext();
        private ProductCategoryDataContext _productCategoryDataContext = new ProductCategoryDataContext();

        [HttpGet]
        public ActionResult ShowImage(int id)
        {
            var p = _productDataContext.Entity.FirstOrDefault(x => x.ProductID == id);
            return base.File(p.ThumbNailPhoto, "image/gif", p.ThumbnailPhotoFileName);
        }

        /*[HttpGet]
        public string GetProductCategories()
        {
            var qwe = _productCategoryDataContext.Entity.Select(x => new {name = x.Name}).ToList();
            //return Json(new {Name = qwe}, JsonRequestBehavior.AllowGet);
            return JsonConvert.SerializeObject(qwe);
        }*/
    }
}
