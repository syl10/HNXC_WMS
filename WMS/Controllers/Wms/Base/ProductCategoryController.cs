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
    public class ProductCategoryController : Controller
    {
        [Dependency]
        public ICMDProductCategoryService ProductCategoryService { get; set; }

        //
        // GET: /ProductCategory/

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

        // GET: /ProductCategory/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string CATEGORY_NAME = collection["CATEGORY_NAME"] ?? "";
            string MEMO = collection["MEMO"] ?? "";
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.isbase = true;
            }
            else
            {
                THOK.Common.PrintHandle.isbase = false;
            }
            //string username = collection["username"] ?? "";
            var users = ProductCategoryService.GetDetails(page, rows, CATEGORY_NAME, MEMO);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /ProductCategory/Create/
        [HttpPost]
        public ActionResult Create(string CATEGORY_NAME, string MEMO)
        {
            bool bResult = ProductCategoryService.Add(CATEGORY_NAME, MEMO);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /ProductCategory/Edit/
        [HttpPost]
        public ActionResult Edit(string CATEGORY_CODE, string CATEGORY_NAME, string MEMO)
        {
            bool bResult = ProductCategoryService.Save(CATEGORY_CODE, CATEGORY_NAME, MEMO);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /ProductCategory/Delete/
        [HttpPost]
        public ActionResult Delete(string CATEGORY_CODE)
        {
            bool bResult = ProductCategoryService.Delete(CATEGORY_CODE);
            string msg = bResult ? "删除成功" : "无法删除,可能原因是正被使用中";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }


    }
}


