using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;

namespace WMS.Controllers.ServerAdmin
{
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
            return Json(users, "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /City/Create/
        [HttpPost]
        public ActionResult Create(string CITY_NAME, string DESCRIPTION, string IS_ACTIVE)
        {
            bool bResult = CityService.Add(CITY_NAME, DESCRIPTION, IS_ACTIVE);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
       
        // POST: /City/Edit/
        [HttpPost]
        public ActionResult Edit(string CITY_ID, string CITY_NAME, string DESCRIPTION, string IS_ACTIVE)
        {
            bool bResult = CityService.Save(CITY_ID, CITY_NAME, DESCRIPTION, IS_ACTIVE);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /City/Delete/
        [HttpPost]
        public ActionResult Delete(string CITY_ID)
        {
            bool bResult = CityService.Delete(CITY_ID);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        // GET: /City/GetDetailsCity/
        public ActionResult GetDetailsCity()
        {
            string cityId = this.GetCookieValue("cityid");
            string userName = this.User.Identity.Name;
            string systemId = this.GetCookieValue("systemid");
            var users = CityService.GetDetails(userName, cityId, systemId);
            return Json(users, "text", JsonRequestBehavior.AllowGet);
        }
        
    }
}
