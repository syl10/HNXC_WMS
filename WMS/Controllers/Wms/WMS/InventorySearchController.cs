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
    public class InventorySearchController : Controller
    {
        //
        // GET: /InventorySearch/
        [Dependency]
        public IWMSBillMasterHService BillMasterService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        //
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
            string BILLNOFROM = collection["BILLNOFROM"] ?? "";  //单号区间 开始部分
            string BILLNOTO = collection["BILLNOTO"] ?? ""; //单号区间  结尾部分
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.issearch = true;
            }
            else
            {
                THOK.Common.PrintHandle.issearch = false;
            }
            var Billmaster = BillMasterService.GetDetails(page, rows, "4", flag, BILL_NO, BILL_DATE, BTYPE_CODE, WAREHOUSE_CODE, BILL_METHOD, CIGARETTE_CODE, FORMULA_CODE, STATE, OPERATER, OPERATE_DATE, CHECKER, CHECK_DATE, STATUS, BILL_DATEStar, BILL_DATEEND, SOURCE_BILLNO,"",BILLNOFROM ,BILLNOTO );
            return Json(Billmaster, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
