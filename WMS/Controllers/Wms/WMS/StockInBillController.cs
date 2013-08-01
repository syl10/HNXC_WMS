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
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            var Billmaster = BillMasterService.GetDetails(page, rows,"1");
            return Json(Billmaster, "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubDetail(int page, int rows, string BillNo)
        {
            var Billdetail = BillMasterService.GetSubDetails (page, rows,BillNo);
            return Json(Billdetail, "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(WMS_BILL_MASTER  mast, object detail)
        {
            bool bResult = BillMasterService.Add(mast, detail);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(WMS_BILL_MASTER  mast, object detail)
        {
            bool bResult = BillMasterService.Edit(mast, detail);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        //单据编号
        public ActionResult GetBillNo(System.DateTime dtime, string BILL_NO)
        {
            string userName = this.GetCookieValue("username");
            var BillnoInfo = BillMasterService.GetBillNo(userName, dtime, BILL_NO );

            return Json(BillnoInfo, "text", JsonRequestBehavior.AllowGet);
        }
        //获取仓库信息
        public ActionResult GetWarehousecode(int page, int rows)
        {
            var warehouse = WarehouseService.GetDetails(page, rows, null);
            return Json(warehouse, "text", JsonRequestBehavior.AllowGet);
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
            bool Result = BillMasterService.Antitrial ( BillNo);
            string msg = Result ? "反审成功" : "反审失败";
            return Json(JsonMessageHelper.getJsonMessage(Result, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        //批次入库时,载入配方.
        public ActionResult LoadFormula(int page, int rows, string Formulacode, decimal BATCH_WEIGHT)
        {
            var Billdetail = BillMasterService.LoadFormulaDetail(page, rows, Formulacode, BATCH_WEIGHT);
            return Json(Billdetail, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
