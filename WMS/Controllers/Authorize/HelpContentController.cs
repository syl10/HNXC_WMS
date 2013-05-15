using System.Web.Mvc;
using System.Text;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.DbModel;
using THOK.Authority.Bll.Interfaces;
using System;

namespace WMS.Controllers.Authority
{
    public class HelpContentController : Controller
    {
        [Dependency]
        public IHelpContentService HelpContentService { get; set; }

        //
        // GET: /HelpContent/
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
        //
        // POST: /HelpContent/Create
        [HttpPost]
        public ActionResult Create(AUTH_HELP_CONTENT helpContent)
        {
            string strResult = string.Empty;
            bool bResult = HelpContentService.Add(helpContent, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /HelpContent/Details/

        public ActionResult Details(int page, int rows, string QueryString, string Value)
        {
            if (QueryString == null)
            {
                QueryString = "ContentName";
            }
            if (Value == null)
            {
                Value = "";
            }
            var product = HelpContentService.GetDetails(page, rows, QueryString, Value);
            return Json(product, "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /HelpContent/Details/

        public ActionResult Details2(int page, int rows, FormCollection collection)
        {
            string ContentCode = collection["ContentCode"] ?? "";
            string ContentName = collection["ContentName"] ?? "";
            string NodeType = collection["NodeType"] ?? "";
            string FatherNodeID = collection["FatherContentID"] ?? "";
            string ModuleID = collection["ModuleID"] ?? "";
            string IsActive = collection["IsActive"] ?? "";;
            var users = HelpContentService.GetDetails2(page, rows, ContentCode, ContentName, NodeType, FatherNodeID, ModuleID, IsActive);
            return Json(users, "text", JsonRequestBehavior.AllowGet);
        }
        // POST: /HelpContent/Edit/
        [HttpPost]
        public ActionResult Edit(string ID, string ContentCode, string ContentName, string ContentPath, string NodeType , string FatherNodeID,string ModuleID,int NodeOrder, string IsActive)
        {
            string strResult = string.Empty;
            bool bResult = HelpContentService.Save(ID, ContentCode, ContentName, ContentPath, FatherNodeID, ModuleID, NodeOrder, IsActive, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /UnitList/Delete/

        [HttpPost]
        public ActionResult Delete(string ID)
        {
            bool bResult = HelpContentService.Delete(ID);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /HelpContent/Help/

        public ActionResult Help(string helpId)
        {
            var help = HelpContentService.Help(helpId);
            return Json(help, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
