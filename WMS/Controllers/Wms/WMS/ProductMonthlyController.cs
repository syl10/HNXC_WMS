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
    public class ProductMonthlyController : Controller
    {
        //
        // GET: /ProductMonthly/
        [Dependency]
        public IWMSBalanceMasterService  BalanceMasterService { get; set; }
        [Dependency]
        public IWMSBalanceDetailService BalanceDetailService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.hasAudit = true;
            ViewBag.hasAntiTrial = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string BALANCENO = collection["BALANCE_NO"] ?? "";//月结号
            string BALANCEDATE = collection["BALANCE_DATE"] ?? "";//月结日期
            string STATE = collection["STATE"] ?? ""; //状态
            string OPERATER = collection["OPERATER"] ?? ""; //操作人
            string CHECKER = collection["CHECKER"] ?? ""; //审核人
            string CHECKDATE = collection["CHECK_DATE"] ?? ""; //审核日期
            var productmonthly = BalanceMasterService.GetDetails(page, rows,BALANCENO ,BALANCEDATE ,STATE ,OPERATER ,CHECKER ,CHECKDATE );
            return Json(productmonthly, "text/html", JsonRequestBehavior.AllowGet);
        }
        //明细
        public ActionResult GetSubDetail(int page, int rows, string Balanceno)
        {
            var Balancedetail = BalanceDetailService.GetSubDetails(page, rows, Balanceno);
            return Json(Balancedetail, "text/html", JsonRequestBehavior.AllowGet);
        }
        //审核
        public ActionResult Audit(string BALANCENO) {
            string checker = this.GetCookieValue("userid");
            bool Result = BalanceMasterService.Audit(checker, BALANCENO);
            string msg = Result ? "审核成功" : "审核失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //反审
        public ActionResult Antitrial(string BALANCENO)
        {
            bool Result = BalanceMasterService.Antitrial(BALANCENO);
            string msg = Result ? "反审成功" : "反审失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //单据编号
        public ActionResult GetBalanceNo()
        {
            string userName = this.GetCookieValue("username");
            var balanceno = new { no=userName};
            return Json(balanceno, "text/html", JsonRequestBehavior.AllowGet);
        }
        //月结
        public ActionResult Balance(string balanceno, string dt)
        {
            string userName = this.GetCookieValue("userid");
            string error = "";
            bool result = BalanceMasterService.Balance(balanceno, DateTime.Parse(dt),userName ,out error );
            string msg = result ? "月结完成" : "月结失败" + error;
            var just = new
            {
                success = msg
            };
            return Json(just, "text/html", JsonRequestBehavior.AllowGet);
        }
        //打印
        public ActionResult Print( string BEGINMONTH, string ENDMONTH, string STATE, string BALANCENO)
        {
            //string Path = Server.MapPath("/");
            string userName = this.GetCookieValue("username");
            bool Result = BalanceMasterService.BalancePrint(BEGINMONTH, ENDMONTH, STATE, BALANCENO);
            string msg = Result ? "成功" : "失败";
            var just = new
            {
                success = msg
            };
            return Json(just, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
