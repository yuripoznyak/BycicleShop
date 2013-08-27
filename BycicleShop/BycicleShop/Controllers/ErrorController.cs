using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BycicleShop.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult HttpError404(string error)
        {
            ViewBag.Description = error;
            return View();
        }

        public ActionResult HttpError403(string error)
        {
            ViewBag.Description = error;
            return View();
        }

        public ActionResult HttpError500(string error)
        {
            ViewBag.Description = error;
            return View();
        }

        public ActionResult Error(string error)
        {
            ViewBag.Description = error;
            return View();
        }
    }
}
