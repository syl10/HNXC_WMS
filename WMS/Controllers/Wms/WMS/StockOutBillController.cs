using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.WebUtil;

namespace WMS.Controllers.Wms.WMS
{
    public class StockOutBillController : Controller
    {
        //
        // GET: /StockOutBill/
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
        public ActionResult Details(int page, int rows,string flag, FormCollection collection)
        {
            var Billmaster = BillMasterService.GetDetails(page, rows,"2",flag ,"","","","","","","","","","","","");
            return Json(Billmaster, "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubDetail(int page, int rows, string BillNo)
        {
            var Billdetail = BillMasterService.GetSubDetails(page, rows, BillNo,0);
            return Json(Billdetail, "text", JsonRequestBehavior.AllowGet);
        }
        //审核
        public ActionResult Audit(string BillNo)
        {
            string checker = this.GetCookieValue("username");
            bool Result = BillMasterService.Audit(checker, BillNo);
            string msg = Result ? "审核成功" : "审核失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        //反审
        public ActionResult Antitrial(string BillNo)
        {
            bool Result = BillMasterService.Antitrial(BillNo);
            string msg = Result ? "反审成功" : "反审失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
    }
}
