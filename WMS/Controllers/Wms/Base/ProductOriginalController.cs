using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.WebUtil;
using THOK.Security;

namespace WMS.Controllers.Wms.Base
{
    [TokenAclAuthorize]
    public class ProductOriginalController : Controller
    {
        //
        // GET: /ProductOriginal/
        [Dependency]
        public ICMDProductOriginalService ProductOriginalService { get; set; }

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
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string ORIGINAL_NAME = collection["ORIGINAL_NAME"] ?? "";
            string DISTRICT_CODE = collection["DISTRICT_CODE"] ?? "";
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
            var originals = ProductOriginalService.Detail(page, rows, ORIGINAL_NAME ,DISTRICT_CODE ,MEMO );
            return Json(originals, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(CMD_PRODUCT_ORIGINAL original)
        {
            bool bResult = ProductOriginalService.Add(original);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(CMD_PRODUCT_ORIGINAL original, string ORIGINAL_CODE)
        {
            bool bResult = ProductOriginalService.Edit(original, ORIGINAL_CODE);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //删除
        public ActionResult Delete(string ORIGINAL_CODE)
        {
            bool bResult = ProductOriginalService.Delete(ORIGINAL_CODE);
            string msg = bResult ? "删除成功" : "无法删除,可能原因是正被使用中";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
