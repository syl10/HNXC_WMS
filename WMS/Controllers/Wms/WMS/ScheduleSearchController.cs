using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Security;
using Wms.Security;

namespace WMS.Controllers.Wms.WMS
{
     [TokenAclAuthorize]
     [SystemEventLog]
    public class ScheduleSearchController : Controller
    {
        //
        // GET: /ScheduleSearch/
        [Dependency]
        public IWMSScheduleMasterService ScheduleMasterService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult GetDetail(int page, int rows, FormCollection collection)
        {
            string SCHEDULE_NO = collection["SCHEDULE_NO"] ?? "";
            string SCHEDULE_DATE = collection["SCHEDULE_DATE"] ?? "";
            string STATE = collection["STATE"] ?? "";
            string OPERATER = collection["OPERATER"] ?? "";
            string OPERATE_DATE = collection["OPERATE_DATE"] ?? "";
            string CHECKER = collection["CHECKER"] ?? "";
            string CHECK_DATE = collection["CHECK_DATE"] ?? "";
            string BILLNOFROM = collection["BILLNOFROM"] ?? "";  //单号区间 开始部分
            string BILLNOTO = collection["BILLNOTO"] ?? ""; //单号区间  结尾部分
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.issearch  = true;
            }
            else
            {
                THOK.Common.PrintHandle.issearch = false;
            }
            var Schedules = ScheduleMasterService.GetDetails(page, rows, SCHEDULE_NO, SCHEDULE_DATE, STATE, OPERATER, OPERATE_DATE, CHECKER, CHECK_DATE,BILLNOFROM,BILLNOTO);
            return Json(Schedules, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
