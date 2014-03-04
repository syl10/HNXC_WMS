using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using Wms.Security;
using THOK.Security;

namespace WMS.Controllers.Authority
{
    [TokenAclAuthorize]
    [SystemEventLog]
    public class LoginLogController : Controller
    {
        [Dependency]
        public ILoginLogService LoginLogService { get; set; }
        //
        // GET: /LoginLog/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        
         //GET: /LoginLog/Details/

        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string SystemID = collection["SystemID"] ?? "";
            string UserID = collection["UserID"] ?? "";
            string LoginPC = collection["LoginPC"] ?? "";
            string LoginTime = collection["LoginTime"] ?? "";
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.isbase = true;
            }
            else
            {
                THOK.Common.PrintHandle.isbase = false;
            }
            var users = LoginLogService.GetDetails(page, rows, SystemID, UserID, LoginPC,LoginTime);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /LoginLog/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /LoginLog/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /LoginLog/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /LoginLog/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /LoginLog/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /LoginLog/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
