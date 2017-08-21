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
    public class UnitCategoryController : Controller
    {
        //
        // GET: /UnitCategory/

        [Dependency]
        public ICMDUnitCategoryService UnitCategoryService { get; set; }


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

        // GET: /UnitCategory/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string CATEGORY_NAME = collection["CATEGORY_NAME"] ?? "";
            string MEMO = collection["MEMO"] ?? "";
            //string username = collection["username"] ?? "";
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.isbase = true;
            }
            else
            {
                THOK.Common.PrintHandle.isbase = false;
            }
            var users = UnitCategoryService.GetDetails(page, rows, CATEGORY_NAME, MEMO);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /UnitCategory/Create/
        [HttpPost]
        public ActionResult Create(CMD_UNIT_CATEGORY UnitCategory)
        {
            bool bResult = UnitCategoryService.Add(UnitCategory);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /UnitCategory/Edit/
        [HttpPost]
        public ActionResult Edit(CMD_UNIT_CATEGORY UnitCategory)
        {
            bool bResult = UnitCategoryService.Save(UnitCategory);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /UnitCategory/Delete/
        [HttpPost]
        public ActionResult Delete(string CATEGORY_CODE)
        {
            bool bResult = UnitCategoryService.Delete(CATEGORY_CODE);
            string msg = bResult ? "删除成功" : "无法删除,可能原因是正被使用中";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //  /LoginLog/CreateExcelToClient/
        //public FileStreamResult CreateExcelToClient()
        //{
        //    int page = 0, rows = 0;
        //    string CIGARETTE_NAME = Request.QueryString["CIGARETTE_NAME"];
        //    string CIGARETTE_MEMO = Request.QueryString["CIGARETTE_MEMO"];

        //    THOK.NPOI.Models.ExportParam ep = new THOK.NPOI.Models.ExportParam();
        //    ep.DT1 = CigaretteService.GetCityExcel(page, rows, CIGARETTE_NAME, CIGARETTE_MEMO);
        //    ep.HeadTitle1 = "牌号信息";
        //    ep.BigHeadColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
        //    ep.ColHeadColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
        //    ep.ContentColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
        //    System.IO.MemoryStream ms = THOK.NPOI.Service.ExportExcel.ExportDT(ep);
        //    return new FileStreamResult(ms, "application/ms-excel");
        //} 

    }
}
