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
    public class WorkSearchController : Controller
    {
        //
        // GET: /WorkSearch/
        [Dependency]
        public IWMSBillMasterService BillMasterService { get; set; }
        [Dependency]
        public IWMSProductStateService ProductStateService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string BILL_NO = collection["BILL_NO"] ?? "";  //单据编号
            string TASK_DATE = collection["TASK_DATE"] ?? ""; //单据日期(出,入库日期)
            string BTYPE_CODE = collection["BTYPE_CODE"] ?? ""; //单据类型
            string TASK_NO = collection["TASK_NO"] ?? ""; //入库方式
            string CIGARETTE_CODE = collection["CIGARETTE_CODE"] ?? ""; //牌号
            string FORMULA_CODE = collection["FORMULA_CODE"] ?? ""; //配方代码
            string PRODUCT_BARCODE = collection["PRODUCT_BARCODE"] ?? ""; //条码
            var worksearch = ProductStateService.Worksearch(page, rows, BILL_NO, TASK_DATE, BTYPE_CODE, TASK_NO, CIGARETTE_CODE, FORMULA_CODE, PRODUCT_BARCODE);
            return Json(worksearch, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubDetail(int page, int rows, string TASK_ID)
        {
            var detail = ProductStateService.Workdetail(page, rows, TASK_ID);
            return Json(detail, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
