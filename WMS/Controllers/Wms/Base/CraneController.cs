using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using System;
using THOK.Wms.Bll.Interfaces;

namespace WMS.Controllers.Wms.Base
{
    public class CraneController : Controller
    {
        [Dependency]
        public ICMDCraneService CraneService { get; set; }

        // GET: /Crane/
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

        // GET: /Crane/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string CRANE_NAME = collection["CRANE_NAME"] ?? "";
            string CRANE_MEMO = collection["MEMO"] ?? "";
            string IS_ACTIVE = collection["IS_ACTIVE"] ?? "";
            //string username = collection["username"] ?? "";
            var users = CraneService.GetDetails(page, rows, CRANE_NAME, CRANE_MEMO, IS_ACTIVE);
            return Json(users, "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /Crane/Create/
        [HttpPost]
        public ActionResult Create(string CRANE_NAME, string CRANE_MEMO, string IS_ACTIVE)
        {
            bool bResult = CraneService.Add(CRANE_NAME, CRANE_MEMO, IS_ACTIVE);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /Crane/Edit/
        [HttpPost]
        public ActionResult Edit(string CRANE_NO, string CRANE_NAME, string CRANE_MEMO, string IS_ACTIVE)
        {
            bool bResult = CraneService.Save(CRANE_NO, CRANE_NAME, CRANE_MEMO, IS_ACTIVE);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /Crane/Delete/
        [HttpPost]
        public ActionResult Delete(string CRANE_NO)
        {
            bool bResult = CraneService.Delete(CRANE_NO);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
    }
}
