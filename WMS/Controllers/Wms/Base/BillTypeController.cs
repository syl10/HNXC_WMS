using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using System;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.Security;


namespace WMS.Controllers.Wms.Base
{
    [TokenAclAuthorize]
    public class BillTypeController : Controller
    {
        [Dependency]
        public ICmdBillTypeService BillTypeService { get; set; }

        // GET: /BillType/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        // GET: /BillType/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string BTYPE_NAME = collection["BTYPE_NAME"] ?? "";
            string BILL_TYPE = collection["BILL_TYPE"] ?? "";
            string TASK_LEVEL = collection["TASK_LEVEL"] ?? "";
            string Memo = collection["MEMO"] ?? "";
            string TARGET_CODE = collection["TARGET_CODE"] ?? "";
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.isbase = true;
            }
            else
            {
                THOK.Common.PrintHandle.isbase = false;
            }
            var users = BillTypeService.GetDetails(page, rows, BTYPE_NAME, BILL_TYPE, TASK_LEVEL, Memo, TARGET_CODE);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /BillType/Create/
        [HttpPost]
        public ActionResult Create(CMD_BILL_TYPE BillType)
        {
            bool bResult = BillTypeService.Add(BillType);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /BillType/Edit/
        [HttpPost]
        public ActionResult Edit(CMD_BILL_TYPE BillType)
        {
            bool bResult = BillTypeService.Save(BillType);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /BillType/Delete/
        [HttpPost]
        public ActionResult Delete(string BTYPE_CODE)
        {
            bool bResult = BillTypeService.Delete(BTYPE_CODE);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
