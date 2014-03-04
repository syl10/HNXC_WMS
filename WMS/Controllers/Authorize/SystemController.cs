using System.Web.Mvc;
using System.Text;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using Wms.Security;
using THOK.Security;

namespace WMS.Controllers.Authority
{
    [TokenAclAuthorize]
    [SystemEventLog]
    public class SystemController : Controller
    {
        [Dependency]
        public ISystemService SystemService { get; set; }

        // GET: /System/
        public ActionResult Index(string moduleID)
        {
            //ViewBag.hasSearch = true;
            //ViewBag.hasAdd = true;
            //ViewBag.hasEdit = true;
            //ViewBag.hasDelete = true;
            //ViewBag.hasPrint = true;
            //ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        // GET: /System/Details/
        public ActionResult Details(int page, int rows,FormCollection collection)
        {
            string SYSTEM_NAME = collection["SYSTEM_NAME"] ?? "";
            string DESCRIPTION = collection["DESCRIPTION"] ?? "";
            string STATUS = collection["STATUS"] ?? "";
            var systems = SystemService.GetDetails(page, rows, SYSTEM_NAME, DESCRIPTION, STATUS);
            return Json(systems, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /System/Create/
        [HttpPost]
        public ActionResult Create(string SYSTEM_NAME, string DESCRIPTION, string STATUS)
        {
            bool bResult = SystemService.Add(SYSTEM_NAME, DESCRIPTION, STATUS);
            string msg = bResult ? "新增成功": "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /System/Edit/
        [HttpPost]
        public ActionResult Edit(string SYSTEM_ID, string SYSTEM_NAME, string DESCRIPTION, string STATUS)
        {
            bool bResult = SystemService.Save(SYSTEM_ID, SYSTEM_NAME, DESCRIPTION, STATUS);
            string msg = bResult ? "修改成功" : "修改失败" ;
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /System/Delete/
        [HttpPost]
        public ActionResult Delete(string SYSTEM_ID)
        {
            bool bResult = SystemService.Delete(SYSTEM_ID);
            string msg = bResult ? "删除成功": "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // GET: /System/GetDetailsSystem/
        public ActionResult GetDetailsSystem()
        {
            string cityId = this.GetCookieValue("cityid");
            string userName = this.User.Identity.Name;
            string systemId = this.GetCookieValue("systemid");
            var systems = SystemService.GetDetails(userName, systemId, cityId);
            return Json(systems, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
