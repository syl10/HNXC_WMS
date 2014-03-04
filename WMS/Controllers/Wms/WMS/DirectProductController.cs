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
    public class DirectProductController : Controller
    {
        //
        // GET: /DirectProduct/

        [Dependency]
        public IWMSProductionMasterService  ProductionmastService { get; set; }

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

        public ActionResult GetDetail(int page, int rows, FormCollection collection)
        {
            string BILL_NO = collection["BILL_NO"] ?? "";  //单据编号
            string BILL_DATE = collection["BILL_DATE"] ?? ""; //单据日期(出,入库日期)
            string WAREHOUSE_CODE = collection["WAREHOUSE_CODE"] ?? ""; //仓库编号
            string CIGARETTE_CODE = collection["CIGARETTE_CODE"] ?? ""; //牌号
            string FORMULA_CODE = collection["FORMULA_CODE"] ?? ""; //配方代码
            string STATE = collection["STATE"] ?? ""; //状态
            string OPERATER = collection["OPERATER"] ?? ""; //操作人
            string OPERATE_DATE = collection["OPERATE_DATE"] ?? ""; //操作日期
            string CHECKER = collection["CHECKER"] ?? ""; //审核人
            string CHECK_DATE = collection["CHECK_DATE"] ?? ""; //审核日期
            string BILL_DATEStar = collection["BILL_DATEStar"] ?? ""; //起始日期
            string BILL_DATEEND = collection["BILL_DATEEND"] ?? "";//截止日期
            string BILLNOFROM = collection["BILLNOFROM"] ?? "";  //单号区间 开始部分
            string BILLNOTO = collection["BILLNOTO"] ?? ""; //单号区间  结尾部分
            var detail = ProductionmastService.GetDetails(page, rows,BILL_NO,BILL_DATE ,WAREHOUSE_CODE ,CIGARETTE_CODE ,FORMULA_CODE ,STATE ,OPERATER ,OPERATE_DATE ,CHECKER ,CHECK_DATE ,BILL_DATEStar ,BILL_DATEEND,BILLNOFROM ,BILLNOTO  );
            return Json(detail, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubDetail(int page, int rows, string BillNo)
        {
            var Billdetail = ProductionmastService.GetSubDetails(page, rows, BillNo);
            return Json(Billdetail, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(WMS_PRODUCTION_MASTER mast, object detail)
        {
            string userid = this.GetCookieValue("userid");
            mast.OPERATER = userid;
            bool bResult = ProductionmastService.Add(mast, detail);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(string billno)
        {
            bool bResult = ProductionmastService.Delete(billno);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(WMS_PRODUCTION_MASTER mast, object detail)
        {
            bool bResult = ProductionmastService.Edit(mast, detail);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //单据编号
        public ActionResult GetBillNo(System.DateTime dtime, string BILL_NO)
        {
            string userName = this.GetCookieValue("username");
            var BillnoInfo = ProductionmastService.GetBillNo(userName, dtime, BILL_NO);

            return Json(BillnoInfo, "text/html", JsonRequestBehavior.AllowGet);
        }
        //审核
        public ActionResult Audit(string billno)
        {
            string checker = this.GetCookieValue("userid");
            bool Result = ProductionmastService.Audit(checker, billno);
            string msg = Result ? "审核成功" : "审核失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //反审
        public ActionResult Antitrial(string billno)
        {
            bool Result = ProductionmastService.Antitrial(billno);
            string msg = Result ? "反审成功" : "反审失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //打印
        public ActionResult Print(string BILLNO,string WAREHOUSECODE, string CIGARETTECODE, string FORMULACODE, string STATE, string BILLDATEFROM, string BILLDATETO,string  SCHEDULENO,string  IN_BILLNO,string  OUT_BILLNO)
        {
            //string Path = Server.MapPath("/");
            string userName = this.GetCookieValue("username");
            bool Result = ProductionmastService.Print(BILLNO, WAREHOUSECODE, CIGARETTECODE, FORMULACODE, STATE, BILLDATEFROM, BILLDATETO, SCHEDULENO, IN_BILLNO, OUT_BILLNO);
            string msg = Result ? "成功" : "失败";
            var just = new
            {
                success = msg
            };
            return Json(just, "text", JsonRequestBehavior.AllowGet);
        }

    }
}
