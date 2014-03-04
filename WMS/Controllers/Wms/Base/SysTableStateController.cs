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
    public class SysTableStateController : Controller
    {
        [Dependency]
        public ISysTableStateService SysTableStateService { get; set; }
        //
        // GET: /SysTableState/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /SysTableState/Details/
        public ActionResult Details(string TableName, string FieldName)
        {
            
            //string username = collection["username"] ?? "";
            var users = SysTableStateService.GetDetails(TableName, FieldName);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
