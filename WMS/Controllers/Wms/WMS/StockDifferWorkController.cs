using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.WebUtil;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using System.Reflection;

namespace WMS.Controllers.Wms.WMS
{
    public class StockDifferWorkController : Controller
    {
        //
        // GET: /StockDifferWork/
        [Dependency]
        public IWMSBillMasterService BillMasterService { get; set; }
        [Dependency]
        public ICMDCellService CellService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasTask = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
      //  对明细下的记录进行分解
        public ActionResult loaddetail(string billno) {
            var Billdetail = BillMasterService.GetSubDetails(1, 1000, billno, 0);
            return Json(Billdetail, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取对应的带有错误标记的货位号
        public ActionResult geterrorcell( string BillNo, string productcode) {
            var errorcell = CellService.GeterrorCell(BillNo, productcode);
            return Json(errorcell, "text/html", JsonRequestBehavior.AllowGet);
        }
        //清空异常货位上的产品信息
        public ActionResult Clearerrorcell(string Cellcode, string BillNo, string productcode)
        {
            string errormsg="";
            bool bResult = CellService.ClearerrorCell (Cellcode,BillNo ,productcode ,ref errormsg );
            string msg = bResult ? "成功" : errormsg;
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
