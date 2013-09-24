using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.WebUtil;
using THOK.Wms.DbModel;

namespace WMS.Controllers.Wms.WMS
{
    public class StockInBillController : Controller
    {
        //
        // GET: /StockInBill/
        [Dependency]
        public IWMSBillMasterService BillMasterService { get; set; }
        [Dependency]
        public ICMDWarehouseService WarehouseService { get; set; }
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
            ViewBag.hasMix = true;
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
            var Billmaster = BillMasterService.GetDetails(page, rows,"1", flag,
                BILL_NO, BILL_DATE,BTYPE_CODE,WAREHOUSE_CODE,BILL_METHOD,CIGARETTE_CODE ,FORMULA_CODE ,STATE ,OPERATER ,OPERATE_DATE ,CHECKER ,CHECK_DATE,STATUS ,BILL_DATEStar ,BILL_DATEEND ,SOURCE_BILLNO  );
            return Json(Billmaster, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubDetail(int page, int rows, string BillNo)
        {
            var Billdetail = BillMasterService.GetSubDetails (page, rows,BillNo,0);
            return Json(Billdetail, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(WMS_BILL_MASTER mast, object detail, string prefix)
        {
            string userid = this.GetCookieValue("userid");
            mast.OPERATER = userid;
            bool bResult = BillMasterService.Add(mast, detail,prefix);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit(WMS_BILL_MASTER  mast, object detail)
        {
            bool bResult = BillMasterService.Edit(mast, detail);
            string msg = bResult ? "修改成功" : "修改失败";
            JsonResult jsors;
            jsors = Json(JsonMessageHelper.getJsonMessage(bResult, msg), "text/html", JsonRequestBehavior.AllowGet);
            return jsors;
        }
        public ActionResult Delete(string Billno) {
            bool bResult = BillMasterService.Delete(Billno);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //单据编号
        public ActionResult GetBillNo(System.DateTime dtime, string BILL_NO,string prefix)
        {
            string userName = this.GetCookieValue("username");
            var BillnoInfo = BillMasterService.GetBillNo(userName, dtime, BILL_NO,prefix );

            return Json(BillnoInfo, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取仓库信息
        public ActionResult GetWarehousecode(int page, int rows)
        {
            var warehouse = WarehouseService.GetDetails(page, rows, null);
            return Json(warehouse, "text/html", JsonRequestBehavior.AllowGet);
        }
        //审核
        public ActionResult Audit(string BillNo)
        {
            string checker = this.GetCookieValue("userid");
            bool Result = BillMasterService.Audit(checker, BillNo);
            string msg = Result ? "审核成功" : "审核失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //反审
        public ActionResult Antitrial(string BillNo)
        {
            bool Result = BillMasterService.Antitrial ( BillNo);
            string msg = Result ? "反审成功" : "反审失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //批次入库时,载入配方.
        public ActionResult LoadFormula(int page, int rows, string Formulacode, decimal BATCH_WEIGHT)
        {
            var Billdetail = BillMasterService.LoadFormulaDetail(page, rows, Formulacode, BATCH_WEIGHT);
            return Json(Billdetail, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取明细中的序号
        public ActionResult GetSerial(string BILL_NO)
        {
            var rejust = BillMasterService.GetSerial(BILL_NO);
            return Json(rejust, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取该单据下的混装产品
        public ActionResult GetMIXproduct(int page, int rows, string BillNo)
        {
            var Billdetail = BillMasterService.GetSubDetails(page, rows, BillNo, 1);
            return Json(Billdetail, "text/html", JsonRequestBehavior.AllowGet);
        }
        //设置混装
        public ActionResult SetMIX(string BillNo, object detail)
        {
            bool Result = BillMasterService.SetMIX (BillNo,detail);
            string msg = Result ? "设置成功" : "设置失败";
            var just = new { 
                success=msg
            };
            return Json(just, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
