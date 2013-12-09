using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS.Controllers.Wms.WMS
{
    public class StockDifferWorkController : Controller
    {
        //
        // GET: /StockDifferWork/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasTask = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

    }
}
