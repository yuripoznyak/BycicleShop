using System;
using System.Linq;
using System.Web.Mvc;
using BycicleShop.Common.SqlContext.Concrete;

namespace BycicleShop.Controllers
{
    public class ImageController : Controller
    {
        private ProductDataContext _productDataContext = new ProductDataContext();

        [HttpGet]
        public ActionResult Show(int id)
        {
            var p = _productDataContext.Entity.FirstOrDefault(x => x.ProductID == id);
            return base.File(p.ThumbNailPhoto, "image/gif", p.ThumbnailPhotoFileName);
        }


    }
}
