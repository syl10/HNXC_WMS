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

namespace WMS.Controllers.ServerAdmin
{
    [TokenAclAuthorize]
    [SystemEventLog]
    public class ServerController : Controller
    {
        [Dependency]
        public IServerService ServerService { get; set; }

        // GET: /Server/
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

        // GET: /Server/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string SERVER_NAME = collection["SERVER_NAME"] ?? "";
            string DESCRIPTION = collection["DESCRIPTION"] ?? "";
            string URL = collection["Url"] ?? "";
            string IS_ACTIVE = collection["IS_ACTIVE"] ?? "";
            string CITY_ID = collection["CITY_ID"] ?? "";
            var users = ServerService.GetDetails(page, rows, SERVER_NAME, DESCRIPTION, URL, IS_ACTIVE, CITY_ID);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Server/Create
        [HttpPost]
        public ActionResult Create(string SERVER_NAME, string DESCRIPTION, string URL, string IS_ACTIVE, string CITY_CITY_ID)
        {
            bool bResult = ServerService.Add(SERVER_NAME, DESCRIPTION, URL, IS_ACTIVE, CITY_CITY_ID);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Server/Edit/ 
        [HttpPost]
        public ActionResult Edit(string SERVER_ID, string SERVER_NAME, string DESCRIPTION, string URL, string IS_ACTIVE, string CITY_CITY_ID)
        {
            bool bResult = ServerService.Save(SERVER_ID, SERVER_NAME, DESCRIPTION, URL, IS_ACTIVE, CITY_CITY_ID);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Server/Delete/
        [HttpPost]
        public ActionResult Delete(string SERVER_ID)
        {
            bool bResult = ServerService.Delete(SERVER_ID);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // GET: /Server/GetDetailsServer/
        public ActionResult GetDetailsServer()
        {
            string CITY_CITY_ID = this.GetCookieValue("cityid");
            string SERVER_ID = this.GetCookieValue("serverid");
            var server = ServerService.GetDetails(CITY_CITY_ID, SERVER_ID);
            return Json(server, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
