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
    public class FillBillController : Controller
    {
        //
        // GET: /FillBill/
        [Dependency]
        public IWMSBillMasterService BillMasterService { get; set; }
        [Dependency]
        public IWMSProductStateService ProductStateService { get; set; }

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
            ViewBag.hasTask = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        //单据编号
        public ActionResult GetBillNo(System.DateTime dtime, string BILL_NO, string prefix)
        {
            string userName = this.GetCookieValue("username");
            var BillnoInfo = BillMasterService.GetBillNo(userName, dtime, BILL_NO, prefix);

            return Json(BillnoInfo, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取某条单据下的作业任务.
        public ActionResult GetSubdetail(int page, int rows, string billno)
        {
            var productstate = ProductStateService.Details(page, rows, billno);
            return Json(productstate, "text/html", JsonRequestBehavior.AllowGet);
        }
        //查找需要补料的单据
        public ActionResult billselect(int page, int rows, string billmethod, string billno)
        {
            var Billmaster = BillMasterService.billselect(page, rows, billmethod,billno );
            return Json(Billmaster, "text/html", JsonRequestBehavior.AllowGet);
        }
        //查找符合的产品条码
        public ActionResult barcodeselect(int page, int rows, string soursebill)
        {
            var barcode = ProductStateService.Barcodeselect(page, rows, soursebill);
            return Json(barcode, "text/html", JsonRequestBehavior.AllowGet);
        }
        //抽检补料入库单添加
        public ActionResult Add(WMS_BILL_MASTER mast, object detail, string prefix)
        {
            string userid = this.GetCookieValue("userid");
            mast.OPERATER = userid;
            bool bResult = BillMasterService.FillBillAdd (mast, detail, prefix);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //编辑
        public ActionResult Edit(WMS_BILL_MASTER mast, object detail)
        {
            bool bResult = BillMasterService.FillBillEdit (mast, detail);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //删除
        public ActionResult Delete(string Billno)
        {
            bool bResult = BillMasterService.FillBillDelete (Billno);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //作业函数
        public ActionResult Task(string BillNo)
        {
            string userName = this.GetCookieValue("userid");
            bool bResult = BillMasterService.FillBillTask(BillNo, userName);
            string msg = bResult ? "作业成功" : "作业失败" ;
            var just = new
            {
                success = msg
            };
            return Json(just, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
