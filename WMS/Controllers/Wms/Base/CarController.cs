using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using System;
using THOK.Wms.Bll.Interfaces;
using THOK.Security;

namespace WMS.Controllers.Wms.Base
{
    [TokenAclAuthorize]
    public class CarController : Controller
    {
        [Dependency]
        public ICMDCarService CarService { get; set; }

        // GET: /Car/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = false;
            ViewBag.hasEdit = true;
            //ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        // GET: /Car/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string CAR_NAME = collection["CAR_NAME"] ?? "";
            string CAR_MEMO = collection["MEMO"] ?? "";
            string IS_ACTIVE = collection["IS_ACTIVE"] ?? "";
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.isbase = true;
            }
            else {
                THOK.Common.PrintHandle.isbase = false;
            }
            //string username = collection["username"] ?? "";
            var users = CarService.GetDetails(page, rows, CAR_NAME, CAR_MEMO, IS_ACTIVE);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Car/Create/
        [HttpPost]
        public ActionResult Create(string CAR_NAME, string CAR_MEMO, string IS_ACTIVE)
        {
            bool bResult = CarService.Add(CAR_NAME, CAR_MEMO, IS_ACTIVE);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Car/Edit/
        [HttpPost]
        public ActionResult Edit(string CAR_NO, string CAR_NAME, string CAR_MEMO, string IS_ACTIVE)
        {
            bool bResult = CarService.Save(CAR_NO, CAR_NAME, CAR_MEMO, IS_ACTIVE);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Car/Delete/
        [HttpPost]
        public ActionResult Delete(string CAR_NO)
        {
            bool bResult = CarService.Delete(CAR_NO);
            string msg = bResult ? "删除成功" : "无法删除,可能原因是正被使用中";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
