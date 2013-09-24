using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.WebUtil;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;

namespace WMS.Controllers.Wms.WMS
{
    public class StockOutWorkController : Controller
    {
        //
        // GET: /StockOutWork/
        [Dependency]
        public IWMSProductStateService ProductStateService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            //ViewBag.hasBarcode = true;
            ViewBag.hasTask = true;
            //ViewBag.hasExit = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        //获取某条单据下的作业任务.
        public ActionResult GetSubdetail(int page, int rows, string billno)
        {
            var productstate = ProductStateService.Details(page, rows, billno);
            return Json(productstate, "text/html", JsonRequestBehavior.AllowGet);
        }
        //作业 函数
        public ActionResult Task(string BillNo, string cigarettecode, string formulacode, string batchweight)
        {
            string userName = this.GetCookieValue("userid");
            string error = "";
            bool bResult = ProductStateService.Task(BillNo,cigarettecode ,formulacode ,batchweight, userName,out error);
            string msg = bResult ? "作业成功" : "作业失败"+error;
            var just = new
            {
                success = msg
            };
            return Json(just, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
