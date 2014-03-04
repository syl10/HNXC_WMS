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
using THOK.Security;

namespace WMS.Controllers.Wms.WMS
{
     [TokenAclAuthorize]
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
        public ActionResult Clearerrorcell(string BillNo,string Cellcode, string inBillNo, string productcode)
        {
            string errormsg="";
            string userid = this.GetCookieValue("userid");
            bool bResult = CellService.ClearerrorCell (BillNo,Cellcode,inBillNo ,productcode ,userid , ref errormsg );
            string msg = bResult ? "成功清空" : errormsg;
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
         //补录数据
        public ActionResult Completerecoder(WMS_TASKRECORD taskrecord, string BillNo, string indate)
        {
            string errormsg="";
            string userid = this.GetCookieValue("userid");
            taskrecord.ACTION = "1";
            taskrecord.BILL_NO = BillNo;
            DateTime indt =string.IsNullOrEmpty (indate )?DateTime .Now : DateTime.Parse(indate);
            bool bResult = CellService.Completedata(taskrecord, userid,indt,ref errormsg);
            string msg = bResult ? "成功补录" : errormsg;
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult TaskrecordDetail(int page, int rows, string Billno, string productcode)
        {
            var taskrecord = CellService.GetTaskrecordDetail(page, rows, Billno, productcode);
            return Json(taskrecord, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
