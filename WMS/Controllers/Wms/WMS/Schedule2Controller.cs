using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.WebUtil;
using THOK.Wms.DbModel;
using THOK.Security;
using Wms.Security;

namespace WMS.Controllers.Wms.WMS
{
     [TokenAclAuthorize]
     [SystemEventLog]
    public class Schedule2Controller : Controller
    {
        //
        // GET: /Schedule2/
        [Dependency]
        public IWMSScheduleMasterService ScheduleMasterService { get; set; }
        [Dependency]
        public IWMSScheduleDetailService ScheduleDetailService { get; set; }

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
            ViewBag.hasOutstore = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        [HttpPost]
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
            var Schedules = ScheduleMasterService.GetDetails(page, rows, SCHEDULE_NO, SCHEDULE_DATE, STATE, OPERATER, OPERATE_DATE, CHECKER, CHECK_DATE,BILLNOFROM ,BILLNOTO );
            return Json(Schedules, "text/html", JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetSubDetail(int page, int rows, string SCHEDULE_NO)
        {
            var subdetail = ScheduleMasterService.GetSubDetails(page, rows, SCHEDULE_NO);
            return Json(subdetail, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取计划生产单号
        public ActionResult GetScheduleNO(System.DateTime dtime, string SCHEDULE_NO)
        {
            string userName = this.GetCookieValue("username");
            var ScheduleInfo = ScheduleMasterService.GetSchedulno(userName, dtime, SCHEDULE_NO);

            return Json(ScheduleInfo, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(WMS_SCHEDULE_MASTER mast, object detail)
        {
            string userid = this.GetCookieValue("userid");
            mast.OPERATER = userid;
            bool bResult = ScheduleMasterService.Add(mast, detail);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(WMS_SCHEDULE_MASTER mast, object detail)
        {
            bool bResult = ScheduleMasterService.Edit(mast , detail);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string  scheduleno)
        {
            bool bResult = ScheduleMasterService.Delete(scheduleno );
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //审核
        public ActionResult Audit(string scheduleno)
        {
            string checker = this.GetCookieValue("userid");
            bool Result = ScheduleMasterService.Audit(checker, scheduleno);
            string msg = Result ? "审核成功" : "审核失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //反审核
        public ActionResult Antitrial(string scheduleno)
        {
            bool Result = ScheduleMasterService.Antitrial(scheduleno);
            string msg = Result ? "反审成功" : "反审失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //制丝线
        public ActionResult GetLine(int page, int rows) {
            var Lines = ScheduleMasterService.GetProductLine(page, rows);
            return Json(Lines, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取明细中的序号
        public ActionResult GetSerial(string SCHEDULENO)
        {
            var rejust = ScheduleDetailService.GetSerial(SCHEDULENO);
            return Json(rejust, "text/html", JsonRequestBehavior.AllowGet);
        }
        //生成出库单
        public ActionResult CreateOutbill(string scheduleno)
        {
            string userid = this.GetCookieValue("userid");
            var rejust = ScheduleMasterService.CreateOutBill(scheduleno,userid);
            string msg = rejust ? "生成成功" : "生成失败";
            return Json(JsonMessageHelper.getJsonMessage(rejust, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //打印
        public ActionResult Print(string SCHEDULENO, string BILLDATEFROM, string BILLDATETO, string STATE)
        {
            //string Path = Server.MapPath("/");
            string userName = this.GetCookieValue("username");
            bool Result = ScheduleMasterService.SchedulePrint(SCHEDULENO, BILLDATEFROM, BILLDATETO, STATE);
            string msg = Result ? "成功" : "失败";
            var just = new
            {
                success = msg
            };
            return Json(just, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
