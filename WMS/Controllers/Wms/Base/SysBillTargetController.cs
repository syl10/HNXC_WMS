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
    public class SysBillTargetController : Controller
    {
        [Dependency]
        public ISysBillTargetService SysBillTargetService { get; set; }
        //
        // GET: /SysBillTarget/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /SysBillTarget/Details/
        public ActionResult Details()
        {

            //string username = collection["username"] ?? "";
            var users = SysBillTargetService.GetDetails();
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }


    }
}
