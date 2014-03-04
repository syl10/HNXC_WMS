using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using System;
using THOK.Security;
using Wms.Security;

namespace WMS.Controllers.ServerAdmin
{
    [TokenAclAuthorize]
    [SystemEventLog]
    public class CityController : Controller
    {
        [Dependency]
        public ICityService CityService { get; set; }

        // GET: /City/
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

        // GET: /City/Details/
        public ActionResult Details(int page, int rows,FormCollection collection)
        {
            string CITY_NAME = collection["CITY_NAME"] ?? "";
            string DESCRIPTION = collection["DESCRIPTION"] ?? "";
            string IS_ACTIVE = collection["IS_ACTIVE"] ?? "";
            //string username = collection["username"] ?? "";
            var users = CityService.GetDetails(page, rows, CITY_NAME, DESCRIPTION, IS_ACTIVE);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /City/Create/
        [HttpPost]
        public ActionResult Create(string CITY_NAME, string DESCRIPTION, string IS_ACTIVE)
        {
            bool bResult = CityService.Add(CITY_NAME, DESCRIPTION, IS_ACTIVE);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
       
        // POST: /City/Edit/
        [HttpPost]
        public ActionResult Edit(string CITY_ID, string CITY_NAME, string DESCRIPTION, string IS_ACTIVE)
        {
            bool bResult = CityService.Save(CITY_ID, CITY_NAME, DESCRIPTION, IS_ACTIVE);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /City/Delete/
        [HttpPost]
        public ActionResult Delete(string CITY_ID)
        {
            bool bResult = CityService.Delete(CITY_ID);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // GET: /City/GetDetailsCity/
        public ActionResult GetDetailsCity()
        {
            string cityId = this.GetCookieValue("cityid");
            string userName = this.User.Identity.Name;
            string systemId = this.GetCookieValue("systemid");
            var users = CityService.GetDetails(userName, cityId, systemId);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        //  /LoginLog/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string CITY_NAME = Request.QueryString["CITY_NAME"];
            string DESCRIPTION = Request.QueryString["DESCRIPTION"];
            string IS_ACTIVE = Request.QueryString["IS_ACTIVE"];

            THOK.NPOI.Models.ExportParam ep = new THOK.NPOI.Models.ExportParam();
            ep.DT1 = CityService.GetCityExcel(page, rows, CITY_NAME, DESCRIPTION, IS_ACTIVE);
            ep.HeadTitle1 = "城市信息";
            ep.BigHeadColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            ep.ColHeadColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            ep.ContentColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            System.IO.MemoryStream ms = THOK.NPOI.Service.ExportExcel.ExportDT(ep);
            return new FileStreamResult(ms, "application/ms-excel");
        } 
        
    }
}
