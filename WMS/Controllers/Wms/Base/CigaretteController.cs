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
    public class CigaretteController : Controller
    {
        //
        // GET: /Cigarette/

        [Dependency]
        public ICMDCigaretteService CigaretteService { get; set; }


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

        // GET: /Cigarette/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string CIGARETTE_NAME = collection["CIGARETTE_NAME"] ?? "";
            string CIGARETTE_MEMO = collection["CIGARETTE_MEMO"] ?? "";
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
            var users = CigaretteService.GetDetails(page, rows, CIGARETTE_NAME, CIGARETTE_MEMO);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /Cigarette/Create/
        [HttpPost]
        public ActionResult Create(string CIGARETTE_NAME, string CIGARETTE_MEMO)
        {
            bool bResult = CigaretteService.Add(CIGARETTE_NAME, CIGARETTE_MEMO);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /Cigarette/Edit/
        [HttpPost]
        public ActionResult Edit(string CIGARETTE_CODE, string CIGARETTE_NAME, string CIGARETTE_MEMO)
        {
            bool bResult = CigaretteService.Save(CIGARETTE_CODE, CIGARETTE_NAME, CIGARETTE_MEMO);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /Cigarette/Delete/
        [HttpPost]
        public ActionResult Delete(string CIGARETTE_CODE)
        {
            bool bResult = CigaretteService.Delete(CIGARETTE_CODE);
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
