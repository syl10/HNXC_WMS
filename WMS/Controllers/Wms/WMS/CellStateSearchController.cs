using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Security;

namespace WMS.Controllers.Wms.WMS
{
    [TokenAclAuthorize]
    public class CellStateSearchController : Controller
    {
        //
        // GET: /CellStateSearch/
        [Dependency]
        public ICMDCellService CellService { get; set; }

        public ActionResult Index(string moduleID)
        {
            //ViewBag.hasPrint = true;
            //ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        //获取某排的货位状态.
        public ActionResult Getcellstate(string shelfcode) {
            var cell = CellService.GetCellByshell(shelfcode);
            return Json(cell, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取某个货位的信息.
        public ActionResult Getcellinfo(string cellcode) {
            var productinfo = CellService.Getproductbycellcode(cellcode);
            return Json(productinfo, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
