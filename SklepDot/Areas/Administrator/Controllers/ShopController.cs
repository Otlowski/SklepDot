using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SklepDot.Areas.Administrator.Controllers
{
    public class ShopController : Controller
    {
        // GET: Administrator/Shop
        public ActionResult Index()
        {
            return View();
        }
    }
}