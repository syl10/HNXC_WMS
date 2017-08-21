using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using Wms.Security;
using THOK.Security;

namespace WMS.Controllers.Authority
{
    [TokenAclAuthorize]
    [SystemEventLog]
    public class RoleController : Controller
    {
        [Dependency]
        public IRoleService RoleService { get; set; }

        // GET: /Role/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.hasPermissionAdmin = true;
            ViewBag.hasUserAdmin = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        // GET: /Role/Details/
        public ActionResult Details(int page, int rows,FormCollection collection)
        {
            string ROLE_NAME = collection["ROLE_NAME"] ?? "";
            string MEMO = collection["MEMO"] ?? "";
            string IS_LOCK = collection["IS_LOCK"] ?? "";
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.isbase = true;
            }
            else
            {
                THOK.Common.PrintHandle.isbase = false;
            }
            var roles = RoleService.GetDetails(page, rows, ROLE_NAME, MEMO, IS_LOCK);
            return Json(roles, "text/html", JsonRequestBehavior.AllowGet);
        }    

        // POST: /Role/Create/
        [HttpPost]
        public ActionResult Create(string ROLE_NAME, string MEMO, bool IS_LOCK)
        {
            bool bResult = RoleService.Add(ROLE_NAME, MEMO, IS_LOCK);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Role/Edit/
        [HttpPost]
        public ActionResult Edit(string ROLE_ID, string ROLE_NAME, string MEMO, bool IS_LOCK)
        {
            bool bResult = RoleService.Save(ROLE_ID, ROLE_NAME, MEMO, IS_LOCK);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Role/Delete/
        [HttpPost]
        public ActionResult Delete(string ROLE_ID)
        {
            bool bResult = RoleService.Delete(ROLE_ID);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Role/GetRoleUser/
        [HttpPost]
        public ActionResult GetRoleUser(string ROLE_ID)
        {
            var users = RoleService.GetRoleUser(ROLE_ID);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Role/GetUserInfo/
        [HttpPost]
        public ActionResult GetUserInfo(string ROLE_ID)
        {
            var users = RoleService.GetUserInfo(ROLE_ID);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Role/DeleteRoleUser/
        [HttpPost]
        public ActionResult DeleteRoleUser(string roleUserIdStr)
        {
            bool bResult = RoleService.DeleteRoleUser(roleUserIdStr);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Role/AddRoleUser/
        [HttpPost]
        public ActionResult AddRoleUser(string ROLE_ID, string userIDstr)
        {
            bool bResult = RoleService.AddRoleUser(ROLE_ID, userIDstr);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
