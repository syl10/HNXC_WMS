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
    public class ScheduleController : Controller
    {
        //
        // GET: /Schedule/
        [Dependency]
        public IWMSScheduleService ScheduleService { set; get; }

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
        [HttpPost]
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string SCHEDULE_NO = collection["SCHEDULE_NO"] ?? "";
            string SCHEDULE_DATE = collection["SCHEDULE_DATE"] ?? "";
            string CIGARETTE = collection["CIGARETTE"] ?? "";
            string FORMULA_CODE = collection["FORMULA_CODE"] ?? "";
            string QUANTITY = collection["QUANTITY"] ?? "";
            string STATE = collection["STATE"] ?? "";
            string OPERATER = collection["OPERATER"] ?? "";
            string OPERATE_DATE = collection["OPERATE_DATE"] ?? "";
            string CHECKER = collection["CHECKER"] ?? "";
            string CHECK_DATE = collection["CHECK_DATE"] ?? "";
            var Schedules = ScheduleService.GetDetails(page, rows,SCHEDULE_NO,SCHEDULE_DATE ,CIGARETTE ,FORMULA_CODE ,QUANTITY ,STATE ,OPERATER ,OPERATE_DATE ,CHECKER ,CHECK_DATE);
            return Json(Schedules, "text/html", JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult Create(WMS_SCHEDULE  schedule)
        {
           bool bResult = ScheduleService .Add (schedule );
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //编辑
        public ActionResult Edit(WMS_SCHEDULE schedule)
        {
            bool bResult = ScheduleService.Save (schedule);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取计划生产单号
        public ActionResult GetScheduleNO(System.DateTime dtime, string SCHEDULE_NO)
        {
            string userName = this.GetCookieValue("username");
            var ScheduleInfo = ScheduleService.GetSchedulno(userName, dtime, SCHEDULE_NO);

            return Json(ScheduleInfo, "text/html", JsonRequestBehavior.AllowGet);
        }
        //删除
        public ActionResult Delet(WMS_SCHEDULE schedule)
        {
            bool bResult = ScheduleService.Delete(schedule.SCHEDULE_NO);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //审核
        public ActionResult Audit(string scheduleno)
        {
            string checker = this.GetCookieValue("username");
            bool Result = ScheduleService.Audit(checker, scheduleno);
            string msg = Result ? "审核成功" : "审核失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //反审
        public ActionResult Antitrial(string scheduleno)
        {
            bool Result = ScheduleService.Antitrial(scheduleno);
            string msg = Result ? "反审成功" : "反审失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
