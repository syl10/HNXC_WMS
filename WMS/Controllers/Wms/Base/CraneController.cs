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
    public class CraneController : Controller
    {
        [Dependency]
        public ICMDCraneService CraneService { get; set; }

        // GET: /Crane/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            //ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            //ViewBag.hasDelete = true;
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
            var users = CraneService.GetDetails(page, rows, CRANE_NAME, CRANE_MEMO, IS_ACTIVE);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Crane/Create/
        [HttpPost]
        public ActionResult Create(string CRANE_NAME, string CRANE_MEMO, string IS_ACTIVE)
        {
            bool bResult = CraneService.Add(CRANE_NAME, CRANE_MEMO, IS_ACTIVE);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Crane/Edit/
        [HttpPost]
        public ActionResult Edit(string CRANE_NO, string CRANE_NAME, string CRANE_MEMO, string IS_ACTIVE)
        {
            bool bResult = CraneService.Save(CRANE_NO, CRANE_NAME, CRANE_MEMO, IS_ACTIVE);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Crane/Delete/
        [HttpPost]
        public ActionResult Delete(string CRANE_NO)
        {
            bool bResult = CraneService.Delete(CRANE_NO);
            string msg = bResult ? "删除成功" : "无法删除,可能原因是正被使用中";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        //转换成execl;
        public FileStreamResult CreateExcelToClient()
        {
            string tablestructstr = Request.QueryString["tablestructstr"];
            string head = Request.QueryString["heard"];
            THOK.NPOI.Models.ExportParam ep = new THOK.NPOI.Models.ExportParam();
            THOK.Common.PrintHandle.setbaseinfodata(tablestructstr);
                ep.DT1 = THOK.Common.PrintHandle.baseinfoprint;
                ep.HeadTitle1 = head;
                System.IO.MemoryStream ms = THOK.NPOI.Service.ExportExcel.ExportDT(ep);
                return new FileStreamResult(ms, "application/ms-excel");
        } 
    }
}
