using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;

namespace WMS.Controllers.Wms.WMS
{
    public class InventoryController : Controller
    {
        //
        // GET: /Inventory/
        [Dependency]
        public IWMSBillMasterService BillMasterService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.hasAudit = true;
            ViewBag.hasAntiTrial = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        //
        public ActionResult Details(int page, int rows, string flag, FormCollection collection)
        {
            var Billmaster = BillMasterService.GetDetails(page, rows, "4", flag, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            return Json(Billmaster, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
