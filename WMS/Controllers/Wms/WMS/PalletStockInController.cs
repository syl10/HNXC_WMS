﻿using System;
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
    public class PalletStockInController : Controller
    {
        //
        // GET: /PalletStockIn/
        [Dependency]
        public IWMSPalletMasterService  PalletmasterService { get; set; }

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
            ViewBag.hasTask = true;
            //ViewBag.hasAudit = true;
            //ViewBag.hasAntiTrial = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult GetDetail(int page, int rows, FormCollection collection)
        {
            string BILL_NO = collection["BILL_NO"] ?? "";
            string BILL_DATE = collection["BILL_DATE"] ?? "";
            string BTYPE_CODE = collection["BTYPE_CODE"] ?? "";
            string WAREHOUSE_CODE = collection["WAREHOUSE_CODE"] ?? "";
            string TARGET = collection["TARGET"] ?? "";
            string STATE = collection["STATE"] ?? "";
            string OPERATER = collection["OPERATER"] ?? "";
            string OPERATE_DATE = collection["OPERATE_DATE"] ?? "";
            string TASKER = collection["TASKER"] ?? "";
            string TASK_DATE = collection["TASK_DATE"] ?? "";
            var master = PalletmasterService.Details(page, rows,"1",BILL_NO ,BILL_DATE ,BTYPE_CODE ,WAREHOUSE_CODE ,TARGET ,STATE ,OPERATER ,OPERATE_DATE ,TASKER ,TASK_DATE );// 1表示入库类型
            return Json(master, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubDetail(int page, int rows, string BillNo)
        {
            var Billdetail = PalletmasterService.GetSubDetails(page, rows, BillNo);
            return Json(Billdetail, "text/html", JsonRequestBehavior.AllowGet);
        }

        //单据编号
        public ActionResult GetBillNo(System.DateTime dtime, string BILL_NO, string prefix)
        {
            string userName = this.GetCookieValue("username");
            var BillnoInfo = PalletmasterService.GetBillNo(userName, dtime, BILL_NO, prefix);

            return Json(BillnoInfo, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(WMS_PALLET_MASTER  mast, object detail, string prefix)
        {
            string userid = this.GetCookieValue("userid");
            mast.OPERATER = userid;
            bool bResult = PalletmasterService.Add(mast, detail, prefix);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(WMS_PALLET_MASTER mast, object detail)
        {
            bool bResult = PalletmasterService.Edit(mast, detail);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(string Billno)
        {
            bool bResult = PalletmasterService.Delete(Billno);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //作业 函数
        public ActionResult Task(string BillNo)
        {
            string userName = this.GetCookieValue("userid");
            string error = "";
            bool bResult = ProductStateService.Task(BillNo, userName, out error);
            string msg = bResult ? "作业成功" : "作业失败" + error;
            var just = new
            {
                success = msg
            };
            return Json(just, "text/html", JsonRequestBehavior.AllowGet);
        }
        //打印
        public ActionResult Print(string flag,string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string STATE)
        {
            //string Path = Server.MapPath("/");
            string userName = this.GetCookieValue("username");
            bool Result = PalletmasterService.Print(flag,BILLNO, BILLDATEFROM, BILLDATETO, BTYPECODE, STATE);
            string msg = Result ? "成功" : "失败";
            var just = new
            {
                success = msg
            };
            return Json(just, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
