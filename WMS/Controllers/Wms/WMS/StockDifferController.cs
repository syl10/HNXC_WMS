using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.WebUtil;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.Security;
using Wms.Security;

namespace WMS.Controllers.Wms.WMS
{
     [TokenAclAuthorize]
     [SystemEventLog]
    public class StockDifferController : Controller
    {
        //损益单
        // GET: /StockDiffer/
        [Dependency]
        public IWMSBillMasterService BillMasterService { get; set; }
        [Dependency]
        public IBillReportService BillReportService { get; set; }

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
            //ViewBag.hasTask = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult Details(int page, int rows, string flag, FormCollection collection)
        {
            string BILL_NO = collection["BILL_NO"] ?? "";  //单据编号
            string BILL_DATE = collection["BILL_DATE"] ?? ""; //单据日期(出,入库日期)
            string BTYPE_CODE = collection["BTYPE_CODE"] ?? ""; //单据类型
            string WAREHOUSE_CODE = collection["WAREHOUSE_CODE"] ?? ""; //仓库编号
            string BILL_METHOD = collection["BILL_METHOD"] ?? ""; //出,入库方式
            string CIGARETTE_CODE = collection["CIGARETTE_CODE"] ?? ""; //牌号
            string FORMULA_CODE = collection["FORMULA_CODE"] ?? ""; //配方代码
            string STATE = collection["STATE"] ?? ""; //状态
            string OPERATER = collection["OPERATER"] ?? ""; //操作人
            string OPERATE_DATE = collection["OPERATE_DATE"] ?? ""; //操作日期
            string CHECKER = collection["CHECKER"] ?? ""; //审核人
            string CHECK_DATE = collection["CHECK_DATE"] ?? ""; //审核日期
            string STATUS = collection["STATUS"] ?? ""; //单据来源
            string BILL_DATEStar = collection["BILL_DATEStar"] ?? ""; //起始日期
            string BILL_DATEEND = collection["BILL_DATEEND"] ?? "";//截止日期
            string SOURCE_BILLNO = collection["SOURCE_BILLNO"] ?? "";//来源单号
            var Billmaster = BillMasterService.GetDetails(page, rows, "6", flag, BILL_NO, BILL_DATE, BTYPE_CODE, WAREHOUSE_CODE, BILL_METHOD, CIGARETTE_CODE, FORMULA_CODE, STATE, OPERATER, OPERATE_DATE, CHECKER, CHECK_DATE, STATUS, BILL_DATEStar, BILL_DATEEND, SOURCE_BILLNO,"");
            return Json(Billmaster, "text/html", JsonRequestBehavior.AllowGet);
        }
        //单据编号
        public ActionResult GetBillNo(System.DateTime dtime, string BILL_NO, string prefix)
        {
            string userName = this.GetCookieValue("username");
            var BillnoInfo = BillMasterService.GetBillNo(userName, dtime, BILL_NO, prefix);
            return Json(BillnoInfo, "text/html", JsonRequestBehavior.AllowGet);
        }
        //某个单据下的明细.
        public ActionResult Getbillsubdetail(int page, int rows, string BillNo)
        {
            var Billdetail = BillMasterService.GetSubDetails(page, rows, BillNo);
            return Json(Billdetail, "text/html", JsonRequestBehavior.AllowGet);
        }
        //新增
        public ActionResult Add(WMS_BILL_MASTER mast, object detail, string prefix)
        {
            string userid = this.GetCookieValue("userid");
            mast.OPERATER = userid;
            bool bResult = BillMasterService.FeedingAdd(mast, detail, prefix);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //编辑
        public ActionResult Edit(WMS_BILL_MASTER mast, object detail)
        {
            bool bResult = BillMasterService.FeedingEdit(mast, detail);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //删除
        public ActionResult Delete(string Billno)
        {
            bool bResult = BillMasterService.Delete(Billno);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //打印
        public ActionResult Print(string btypecode, string BILLNO, string BILLDATEFROM, string BILLDATETO, string STATE, string SOURSEBILL)
        {
            //string Path = Server.MapPath("/");
            string userName = this.GetCookieValue("username");
            //THOK.Common.PrintHandle.issearch = true;
            bool Result = BillReportService.StockdiffPrint (BILLNO, BILLDATEFROM, BILLDATETO, STATE, SOURSEBILL, btypecode);
            string msg = Result ? "成功" : "失败";
            var just = new
            {
                success = msg
            };
            return Json(just, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
