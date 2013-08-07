using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS.Controllers.Wms.WMS
{
    public class StockInWorkController : Controller
    {
        //
        // GET: /StockInWork/

        public ActionResult Index()
        {
            ViewBag.hasSearch = true;
            return View();
        }

    }
}
