using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using Wms.Security;
using THOK.Security;

namespace Wms.Controllers.Authority
{
    [TokenAclAuthorize]
    [SystemEventLog]
    public class HelpController : Controller
    {
        //
        // GET: /Help/
        [Dependency]
        public IHelpContentService HelpContentService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

    }
}
