using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.WebUtil;

namespace WMS.Controllers.Wms.Base
{
    public class ProductGradeController : Controller
    {
        //
        // GET: /ProductGrade/
        [Dependency]
        public ICMDProductGradeService ProductGradeService { get; set; }
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
            string ENGLISH_CODE = collection["ENGLISH_CODE"] ?? "";
            string USER_CODE = collection["USER_CODE"] ?? "";
            string GRADE_NAME = collection["GRADE_NAME"] ?? "";
            string MEMO = collection["MEMO"] ?? "";
            var grade = ProductGradeService.Detail(page, rows, ENGLISH_CODE, USER_CODE, GRADE_NAME, MEMO);
            return Json(grade, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(CMD_PRODUCT_GRADE  grade)
        {
            bool bResult = ProductGradeService.Add(grade);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(CMD_PRODUCT_GRADE grade, string GRADE_CODE)
        {
            bool bResult = ProductGradeService.Edit(grade, GRADE_CODE );
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //删除
        public ActionResult Delete(string GRADE_CODE)
        {
            bool bResult = ProductGradeService.Delete(GRADE_CODE);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
