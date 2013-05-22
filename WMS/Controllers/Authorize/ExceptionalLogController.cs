using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;


namespace WMS.Controllers.Authorize
{
    public class ExceptionalLogController : Controller
    {
        [Dependency]
        public IExceptionalLogService ExceptionalLogService { get; set; }
        //
        // GET: /ExceptionalLog/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        //
        // GET: /ExceptionalLog/Details/5

        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string CatchTime = collection["CatchTime"] ?? "";
            string ModuleName = collection["ModuleName"] ?? "";
            string FunctionName = collection["FunctionName"] ?? "";
            string ExceptionalType = collection["ExceptionalType"] ?? "";
            var users = ExceptionalLogService.GetDetails(page, rows, CatchTime, ModuleName, FunctionName, ExceptionalType);
            return Json(users, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
