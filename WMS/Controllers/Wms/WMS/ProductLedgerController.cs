using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.WebUtil;
using THOK.Security;
using Wms.Security;

namespace WMS.Controllers.Wms.WMS
{
     [TokenAclAuthorize]
     [SystemEventLog]
    public class ProductLedgerController : Controller
    {
        //产品总账
        // GET: /ProductLedger/
        [Dependency]
        public IWMSBalanceMasterService BalanceMasterService { get; set; }
        [Dependency]
        public IWMSBalanceDetailService BalanceDetailService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        //获取已月结的年月。
        public ActionResult GetBalanceNo() {
            var nos = BalanceMasterService.GetBalanceNo();
            return Json(nos, "text/html", JsonRequestBehavior.AllowGet);
        }
        //产品总账
        public ActionResult Ledger(int page, int rows, string begin, string end)
        {
            var ledger = BalanceDetailService.Ledger(page, rows, begin, end);
            return Json(ledger, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
